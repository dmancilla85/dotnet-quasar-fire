using FuegoDeQuasar.src.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FuegoDeQuasar.Controllers
{
    public class SecretTransmission
    {
        [Required]
        public IEnumerable<SatelliteMessage> Satellites { get; set; }

        /// <summary>
        /// Count total words
        /// </summary>
        /// <returns>Total words</returns>
        public int TotalWords()
        {
            int words = 0;
            foreach (var item in Satellites)
            {
                words = item.Message.Count() > words ? item.Message.Count() : words;
            }

            return words;
        }

        /// <summary>
        /// Return the minimun number of words in the messages
        /// </summary>
        /// <returns>Minimun length of vector Message</returns>
        private int MinimumWords()
        {
            int min = int.MaxValue;
            foreach (var item in Satellites)
            {
                if (min > item.Message.Count())
                {
                    min = item.Message.Count();
                }
            }

            return min;
        }

        /// <summary>
        /// Check lenghts for phase shift
        /// </summary>
        private IEnumerable<SatelliteMessage> CheckLengths()
        {
            List<SatelliteMessage> shifteds = new();
            int min = MinimumWords();

            foreach(var item in Satellites)
            {
                int count = item.Message.Count();
                IEnumerable<string> aux;
                if (count > min)
                {
                    aux = item.Message.ToList().GetRange(count - min, min);

                    shifteds.Add(
                    new SatelliteMessage()
                    {
                        Message = aux,
                        Distance=item.Distance,
                        Name=item.Name
                    });
                }
                else
                {
                    shifteds.Add(item);
                }
            }

            return shifteds;
        }

        /// <summary>
        /// Return the full message
        /// </summary>
        /// <returns>Message completed</returns>
        public string GetMessage()
        {
            IEnumerable<string> mergedMessages = MergeBrokenMessages();
            StringBuilder finalMessage = new();
            int length = MinimumWords();

            for (int i = 0; i < mergedMessages.Count(); i++)
            {
                finalMessage.Append(mergedMessages.ElementAt(i));
                finalMessage.Append(i == mergedMessages.Count() - 1 ? '.' : ' ');
            }

            return finalMessage.Length == length ? finalMessage.ToString():"";
        }

        public IEnumerable<string> MergeBrokenMessages()
        {
            IEnumerable<string> message = Enumerable.Empty<string>();
            IEnumerable<SatelliteMessage> shifted = CheckLengths();
            int length = MinimumWords();
            string aux;

            for (int i = 0; i < length; i++)
            {
                foreach (var item in shifted)
                {
                    aux = item.Message.ElementAt(i);

                    if (aux != "" && (!message.Any() || message.Last() != aux))
                    {
                        message = message.Append(aux);
                    }
                }
            }

            return message;
        }
    }
}