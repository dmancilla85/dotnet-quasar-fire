using System.Collections.Generic;
using System.Text;

namespace FuegoDeQuasar.Model
{
    public class Message
    {
        public readonly IEnumerable<string> words;

        public Message(List<string> words)
        {
            this.words = words;
        }

        public override string ToString()
        {
            StringBuilder message = new StringBuilder();

            foreach (var item in words)
            {
                message.Append(item?.Length == 0 ? "*" : item);
                //consider dots
                message.Append(' ');
            }

            return message.ToString();
        }
    }
}