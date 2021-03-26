using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FuegoDeQuasar.Model.Requests
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
        /// Return the minimun number of words in the messages
        /// </summary>
        /// <returns>Minimun length of vector Message</returns>
        private static int MinimumWords(IEnumerable<SatelliteMessage> satellites)
        {
            int min = int.MaxValue;
            foreach (var item in satellites)
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

            foreach (var item in Satellites)
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
                        Distance = item.Distance,
                        Name = item.Name
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
        /// Check lenghts for phase shift
        /// </summary>
        private static IEnumerable<SatelliteMessage> CheckLengths(IEnumerable<SatelliteMessage> satellites)
        {
            List<SatelliteMessage> shifteds = new();
            int min = MinimumWords(satellites);

            foreach (var item in satellites)
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
                        Distance = item.Distance,
                        Name = item.Name
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

            return mergedMessages.Count() == length ? finalMessage.ToString() : "";
        }

        /// <summary>
        /// Return the full message
        /// </summary>
        /// <returns>Message completed</returns>
        public static string GetMessage(IEnumerable<SatelliteMessage> satellites)
        {
            IEnumerable<string> mergedMessages = MergeBrokenMessages(satellites);
            StringBuilder finalMessage = new();
            int length = MinimumWords(satellites);

            for (int i = 0; i < mergedMessages.Count(); i++)
            {
                finalMessage.Append(mergedMessages.ElementAt(i));
                finalMessage.Append(i == mergedMessages.Count() - 1 ? '.' : ' ');
            }

            return mergedMessages.Count() == length ? finalMessage.ToString() : "";
        }

        private IEnumerable<string> MergeBrokenMessages()
        {
            List<string> message = new();
            IEnumerable<SatelliteMessage> shifted = CheckLengths();
            int length = MinimumWords();
            string aux;

            for (int i = 0; i < length; i++)
            {
                foreach (var item in shifted)
                {
                    aux = item.Message.ElementAt(i);

                    if (aux != "" && (message.Count == 0 || message.Last() != aux))
                    {
                        message.Add(aux);
                    }
                }
            }

            return message;
        }

        private static IEnumerable<string> MergeBrokenMessages(IEnumerable<SatelliteMessage> satellites)
        {
            List<string> message = new();
            IEnumerable<SatelliteMessage> shifted = CheckLengths(satellites);
            int length = MinimumWords(satellites);
            string aux;

            for (int i = 0; i < length; i++)
            {
                foreach (var item in shifted)
                {
                    aux = item.Message.ElementAt(i);

                    if (aux != "" && (message.Count == 0 || message.Last() != aux))
                    {
                        message.Add(aux);
                    }
                }
            }

            return message;
        }
    }
}