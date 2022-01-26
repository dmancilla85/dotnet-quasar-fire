using FuegoDeQuasar.Model.Requests;
using FuegoDeQuasar.Model.Response;
using FuegoDeQuasar.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace FuegoDeQuasar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopSecretSplitController : Controller
    {
        private readonly ILogger<TopSecretController> _logger;
        private readonly ITopSecretService _topSecretService;

        public TopSecretSplitController(ILogger<TopSecretController> logger,
            ITopSecretService topSecretService)
        {
            _logger = logger;
            _topSecretService = topSecretService;
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
            _logger.LogInformation("Validating message...");

            if (!ModelState.IsValid)
            {
                foreach (string item in ModelState
                    .Values
                    .SelectMany(model => model.Errors?.Select(x => x.ErrorMessage)))
                {
                    _logger.LogError(item);
                }

                return BadRequest(ModelState);
            }

            FinalResponse response = _topSecretService.GetFinalResponseFromSplit(satellite, secret);

            if (response == null)
            {
                return NotFound("No se pudieron obtener los resultados");
            }

            return Ok(response);
        }
    }
}