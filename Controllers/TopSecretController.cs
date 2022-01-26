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
    public class TopSecretController : ControllerBase
    {
        private readonly ILogger<TopSecretController> _logger;
        private readonly ITopSecretService _topSecretService;

        public TopSecretController(ILogger<TopSecretController> logger,
                                    ITopSecretService topSecretService)
        {
            _logger = logger;
            _topSecretService = topSecretService;
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

            FinalResponse response = _topSecretService.GetFinalResponse(secret);

            if (response == null)
            {
                return NotFound("No se pudieron obtener los resultados");
            }

            return Ok(response);
        }
    }
}