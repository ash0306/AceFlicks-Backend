using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Theatre
{
    [Serializable]
    public class NoSuchTheatreException : Exception
    {
        public NoSuchTheatreException()
        {
        }

        public NoSuchTheatreException(string? message) : base(message)
        {
        }

        public NoSuchTheatreException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoSuchTheatreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}