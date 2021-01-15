using System;
using System.Runtime.Serialization;

namespace MTController2.Exp2
{
    [Serializable]
    internal class NotAppropriateStateException : Exception
    {
        public NotAppropriateStateException()
        {
        }

        public NotAppropriateStateException(string message) : base(message)
        {
        }

        public NotAppropriateStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotAppropriateStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}