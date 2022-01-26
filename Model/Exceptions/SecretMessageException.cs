using System;
using System.Runtime.Serialization;

namespace FuegoDeQuasar.Model.Exceptions
{
    /// <summary>
    /// Excepción generada por usuario
    /// </summary>
    [Serializable]
    public sealed class SecretMessageException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SecretMessageException(string message) : base(message)
        {
        }

        private SecretMessageException(SerializationInfo info, StreamingContext context)
                            : base(info, context)
        {
            //
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // ...
        }
    }
}