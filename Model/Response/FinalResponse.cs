using System.ComponentModel;
using System.Text.Json;

namespace FuegoDeQuasar.Model.Response
{
    public class FinalResponse
    {
        [DefaultValue("this is the original message")]
        public string Message { get; set; }

        public Point2D Position { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}