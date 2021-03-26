using FuegoDeQuasar.Configuration;
using FuegoDeQuasar.Model;
using FuegoDeQuasar.Model.Interfaces;
using FuegoDeQuasar.Model.Requests;
using FuegoDeQuasar.Model.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace FuegoDeQuasar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopSecretController : ControllerBase
    {
        private readonly ILogger<TopSecretController> _logger;
        private readonly SatellitesOptions _options;

        public TopSecretController(ILogger<TopSecretController> logger,
                                    IOptions<SatellitesOptions> options)
        {
            _logger = logger;
            _options = options?.Value;
        }

        /// <summary>
        /// Recover help secret message and emitter position through the Alliance's satellites.
        /// </summary>
        /// <param name="secret"></param>
        /// <remarks>
        /// <para>
        /// It receives the distance of the sender and an incomplete message to three satellites (Kenobi,Skywalker and Sato). 
        /// If the information is complete, it returns the approximate position of the sender, and the reconstructed message.
        /// </para>
        /// <para>
        /// Sample request:
        ///     POST /topsecret/
        ///     <code>
        ///     "satellites": [
        ///         {
        ///             “name”: "kenobi",
        ///             “distance”: 100.0,
        ///             “message”: ["este", "", "", "mensaje", ""]
        ///         },
        ///         {
        ///             “name”: "skywalker",
        ///             “distance”: 115.5
        ///             “message”: ["", "es", "", "", "secreto"]
        ///         },
        ///         {
        ///             “name”: "sato",
        ///             “distance”: 142.7
        ///             “message”: ["este", "", "un", "", ""]
        ///         }
        ///      ]
        ///     </code>
        /// </para>
        /// </remarks>
        /// <returns> The distance of the emitter and the original message. </returns>
        /// <response code="200">Returns the emitter's position and the original message.</response>
        /// <response code="400">If the body of the request is incomplete.</response>
        /// <response code="404">If there isn't enough information to recover the position or the original message.</response>
        /// <response code="500">If there is a problem to recover the satellites coordinates.</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<FinalResponse> PostMessage([FromBody] SecretTransmission secret)
        {
            IPoint position;
            string message;

            _logger.LogInformation("Validating message...");

            if (!ModelState.IsValid)
            {
                foreach (string item in ModelState.Values.SelectMany(model => model.Errors?.Select(x => x.ErrorMessage)))
                {
                    _logger.LogError(item);
                }

                return BadRequest(ModelState);
            }

            ISatellite kenobi = _options.Satellites.FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "kenobi",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite skywalker = _options.Satellites.FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "skywalker",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite sato = _options.Satellites.FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "sato",
                StringComparison.InvariantCultureIgnoreCase));

            if (kenobi == null || skywalker == null || sato == null)
            {
                _logger.LogCritical("Fatal error: Satellites configuration is missing.");
                return StatusCode(500);
            }

            _logger.LogInformation("Calculating message emitter distance...");

            if (secret.Satellites.Count() < 3)
            {
                _logger.LogError("Can't recover the emitter position.");
                return NotFound("Can't recover the emitter position.");
            }

            position = (Point2D)Point2D.Triangulation(kenobi, secret.Satellites.FirstOrDefault(e => e.Name == "kenobi").Distance,
                                  skywalker, secret.Satellites.FirstOrDefault(e => e.Name == "skywalker").Distance,
                                  sato, secret.Satellites.FirstOrDefault(e => e.Name == "sato").Distance);

            if (position == null)
            {
                _logger.LogError("Can't recover the emitter position.");
                return NotFound("Can't recover the emitter position.");
            }

            _logger.LogInformation($"Emitter approximate position is {position}.");
            _logger.LogInformation($"Approximate distance to Kenobi is {kenobi.DistanceToPoint(position)}");
            _logger.LogInformation($"Approximate distance to SkyWalker is {skywalker.DistanceToPoint(position)}");
            _logger.LogInformation($"Approximate distance to Sato is {sato.DistanceToPoint(position)}");
            _logger.LogInformation("Recovering original message from satellites...");
            message = secret.GetMessage();

            if (message.Length == 0)
            {
                _logger.LogError("Can't recover the original message.");
                return NotFound("Can't recover the original message.");
            }

            _logger.LogInformation($"The secret message is: {message}");

            return Ok(new FinalResponse() { Position = position, Message = message });
        }
    }
}