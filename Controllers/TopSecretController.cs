using FuegoDeQuasar.Configuration;
using FuegoDeQuasar.Model;
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
        /// Test function
        /// </summary>
        /// <returns> A single satellite</returns>
        [HttpGet]
        public ActionResult Get()
        {
            ISatellite kenobi = _options.Satellites.FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "kenobi",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite skywalker = _options.Satellites.FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "skywalker",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite sato = _options.Satellites.FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "sato",
                StringComparison.InvariantCultureIgnoreCase));

            _logger.LogInformation($"{kenobi} is {kenobi.DistanceTo(sato):0.##} meters far from {sato.GetName()}.");
            _logger.LogInformation($"{kenobi} is {kenobi.DistanceTo(skywalker):0.##} meters far from {skywalker.GetName()}.");
            _logger.LogInformation($"{sato} is {sato.DistanceTo(skywalker):0.##} meters far from {skywalker.GetName()}.");
            return Ok("FINE");
        }

        /// <summary>
        /// Recover message and position of the emisor.
        /// </summary>
        /// <param name="secret"></param>
        /// <remarks>
        /// <para>
        /// Sample request:
        ///     POST /topsecret/
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
        /// </para>
        /// </remarks>
        /// <returns> The distance of the emitter and the original message. </returns>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult PostMessage([FromBody] SecretTransmission secret)
        {
            Point2D position;
            string message;

            _logger.LogInformation("Validating message...");

            if (!ModelState.IsValid)
            {
                foreach (string item in ModelState.Values.SelectMany(model => model.Errors?.Select(x => x.ErrorMessage)))
                {
                    _logger.LogInformation(item);
                }

                return BadRequest(ModelState);
            }

            ISatellite kenobi = _options.Satellites.FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "kenobi",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite skywalker = _options.Satellites.FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "skywalker",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite sato = _options.Satellites.FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "sato",
                StringComparison.InvariantCultureIgnoreCase));

            _=kenobi?? throw new NullReferenceException(nameof(kenobi));
            _ = sato ?? throw new NullReferenceException(nameof(kenobi));
            _ = skywalker ?? throw new NullReferenceException(nameof(kenobi));

            _logger.LogInformation("Message is ok.");
            _logger.LogInformation("Calculating message emitter distance...");
            position = (Point2D)Point2D.Triangulation(kenobi, secret.Satellites.FirstOrDefault(e => e.Name == "kenobi").Distance,
                                  skywalker, secret.Satellites.FirstOrDefault(e => e.Name == "skywalker").Distance,
                                  sato, secret.Satellites.FirstOrDefault(e => e.Name == "sato").Distance);
            _logger.LogInformation($"Emitter position is {position}.");
            _logger.LogInformation($"Distance to Kenobi is {kenobi.DistanceToPoint(position)}");
            _logger.LogInformation($"Distance to SkyWalker is {skywalker.DistanceToPoint(position)}");
            _logger.LogInformation($"Distance to Sato is {sato.DistanceToPoint(position)}");
            _logger.LogInformation("Recovering original message from satellites...");
            message = secret.GetMessage();
            _logger.LogInformation($"The secret message is: {message}");

            return Ok(message);
        }
    }
}