using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Theatre
{
    [Serializable]
    internal class NoTheatresFoundException : Exception
    {
        public NoTheatresFoundException()
        {
        }

        public NoTheatresFoundException(string? message) : base(message)
        {
        }

        public NoTheatresFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoTheatresFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}