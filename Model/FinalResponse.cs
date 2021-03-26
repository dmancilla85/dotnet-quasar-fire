using System.Text.Json;

namespace FuegoDeQuasar.Model
{
    public class FinalResponse
    {
        public IPoint Position { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}