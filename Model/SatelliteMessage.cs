using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;

namespace FuegoDeQuasar.src.Model
{
    public class SatelliteMessage
    {
        [Required]
        public double Distance { get; set; }

        [Required]
        public IEnumerable<string> Message { get; set; }

        [Required]
        public string Name { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public int TotalWords()
        {
            return Message.Count(e => e.Length != 0);
        }
    }
}