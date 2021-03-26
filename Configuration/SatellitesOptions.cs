using FuegoDeQuasar.Model;
using System.Collections.Generic;

namespace FuegoDeQuasar.Configuration
{
    public class SatellitesOptions
    {
        public const string SatellitesConfiguration = "SatellitesConfiguration";
        public IEnumerable<Satellite> Satellites { get; set; }
    }
}