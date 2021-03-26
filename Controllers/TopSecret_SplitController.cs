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
using System.Collections.Generic;
using System.Linq;

namespace FuegoDeQuasar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopSecret_SplitController : Controller
    {
        private readonly ILogger<TopSecretController> _logger;
        private readonly SatellitesOptions _options;
        private readonly static List<SatelliteMessage> Satellites = new List<SatelliteMessage>();

        public TopSecret_SplitController(ILogger<TopSecretController> logger,
                                    IOptions<SatellitesOptions> options)
        {
            _logger = logger;
            _options = options?.Value;
        }

        /// <summary>
        /// Recover help secret message and emitter position, sending distance and message pieces by satellite.
        /// </summary>
        /// <param name="satellite"></param>
        /// <param name="secret"></param>
        /// <remarks>
        /// <para>
        /// Receive the distance to a specific satellite and its incomplete message. If the information 
        /// from all three satellites is available, it will try to obtain the position by triangulation 
        /// and retrieve the original message. Each new message takes the place of the previous one on 
        /// the respective satellite.
        /// </para>
        /// Sample request:
        ///     POST /topsecret_split/[satellite_name]
        /// <code>
        ///      {
        ///          “distance”: 100.0,
        ///          “message”: ["este", "", "", "mensaje", ""]
        ///      }
        /// </code>
        /// </remarks>
        /// <returns> The distance of the emitter and the original message. </returns>
        /// <response code="200">Returns the emitter's position and the original message.</response>
        /// <response code="400">If the body of the request is incomplete.</response>
        /// <response code="404">If there isn't enough information to recover the position or the original message.</response>
        /// <response code="500">If there is a problem to recover the satellites coordinates.</response>
        [HttpPost("{satellite}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<FinalResponse> PostMessage([FromRoute] string satellite, [FromBody] SecretTransmissionSplit secret)
        {
            Point2D position;
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

            SatelliteMessage msg = Satellites.Find(s => string.Equals(s.Name.ToLowerInvariant(), satellite,
                StringComparison.InvariantCultureIgnoreCase));

            if (msg == null)
            {
                if (satellite != kenobi.GetName() && satellite != skywalker.GetName() &&
                satellite != sato.GetName())
                {
                    return BadRequest("The reported satellite does not exist.");
                }
                else
                {
                    Satellites.Add(new SatelliteMessage() { Distance = secret.Distance, Message = secret.Message, Name = satellite });
                }
            }
            else
            {
                msg.Message = secret.Message;
                msg.Distance = secret.Distance;
            }

            if (Satellites.Count < 3)
            {
                _logger.LogError("There's enough information to recover the emitter position and original message.");
                return NotFound("There's enough information to recover the emitter position and original message.");
            }

            _logger.LogInformation("Calculating message emitter distance...");
            position = Point2D.Triangulation(kenobi, Satellites.Find(e => e.Name == "kenobi").Distance,
                                  skywalker, Satellites.Find(e => e.Name == "skywalker").Distance,
                                  sato, Satellites.Find(e => e.Name == "sato").Distance);

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
            message = SecretTransmission.GetMessage(Satellites);

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