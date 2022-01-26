using FuegoDeQuasar.Configuration;
using FuegoDeQuasar.Model;
using FuegoDeQuasar.Model.Exceptions;
using FuegoDeQuasar.Model.Interfaces;
using FuegoDeQuasar.Model.Requests;
using FuegoDeQuasar.Model.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FuegoDeQuasar.Services
{
    public class TopSecretService : ITopSecretService
    {
        private static readonly List<SatelliteMessage> Satellites = new();
        private readonly ILogger<ITopSecretService> _logger;
        private readonly SatellitesOptions _options;

        public TopSecretService(ILogger<ITopSecretService> logger,
                                    IOptions<SatellitesOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public FinalResponse GetFinalResponse(SecretTransmission secret)
        {
            Point2D position;
            string message;

            ISatellite kenobi = _options.Satellites
                .FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "kenobi",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite skywalker = _options.Satellites
                .FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "skywalker",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite sato = _options.Satellites
                .FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "sato",
                StringComparison.InvariantCultureIgnoreCase));

            if (kenobi == null || skywalker == null || sato == null)
            {
                _logger.LogCritical("Fatal error: Satellites configuration is missing.");
                throw new SecretMessageException("Fatal error: Satellites configuration is missing.");
            }

            _logger.LogInformation("Calculating message emitter distance...");

            if (secret.Satellites.Count() < 3)
            {
                _logger.LogError("Can't recover the emitter position.");
                return null;
            }

            position = Point2D.Triangulation(kenobi, secret.Satellites.FirstOrDefault(e => e.Name == "kenobi").Distance,
                                  skywalker, secret.Satellites.FirstOrDefault(e => e.Name == "skywalker").Distance,
                                  sato, secret.Satellites.FirstOrDefault(e => e.Name == "sato").Distance);

            if (position == null)
            {
                _logger.LogError("Can't recover the emitter position.");
                return null;
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
                return null;
            }

            _logger.LogInformation($"The secret message is: {message}");

            return new FinalResponse() { Position = position, Message = message };
        }

        public FinalResponse GetFinalResponseFromSplit(string satellite, SecretTransmissionSplit secret)
        {
            Point2D position;
            string message;

            ISatellite kenobi = _options.Satellites
                .FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "kenobi",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite skywalker = _options.Satellites
                .FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "skywalker",
                StringComparison.InvariantCultureIgnoreCase));
            ISatellite sato = _options.Satellites
                .FirstOrDefault(s => string.Equals(s.Name.ToLowerInvariant(), "sato",
                StringComparison.InvariantCultureIgnoreCase));

            if (kenobi == null || skywalker == null || sato == null)
            {
                _logger.LogCritical("Fatal error: Satellites configuration is missing.");
                throw new SecretMessageException("Fatal error: Satellites configuration is missing.");
            }

            SatelliteMessage msg = Satellites.Find(s => string.Equals(s.Name.ToLowerInvariant(), satellite,
                StringComparison.InvariantCultureIgnoreCase));

            if (msg == null)
            {
                if (satellite != kenobi.GetName() && satellite != skywalker.GetName() &&
                satellite != sato.GetName())
                {
                    _logger.LogError("The reported satellite does not exist.");
                    return null;
                }
                else
                {
                    Satellites.Add(new SatelliteMessage()
                    {
                        Distance = secret.Distance,
                        Message = secret.Message,
                        Name = satellite
                    });
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
                return null;
            }

            _logger.LogInformation("Calculating message emitter distance...");
            position = Point2D.Triangulation(kenobi, Satellites.Find(e => e.Name == "kenobi").Distance,
                                  skywalker, Satellites.Find(e => e.Name == "skywalker").Distance,
                                  sato, Satellites.Find(e => e.Name == "sato").Distance);

            if (position == null)
            {
                _logger.LogError("Can't recover the emitter position.");
                return null;
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
                return null;
            }

            _logger.LogInformation($"The secret message is: {message}");

            return new FinalResponse() { Position = position, Message = message };
        }
    }
}
