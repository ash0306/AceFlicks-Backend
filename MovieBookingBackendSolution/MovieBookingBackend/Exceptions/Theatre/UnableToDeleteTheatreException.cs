using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Theatre
{
    [Serializable]
    public class UnableToDeleteTheatreException : Exception
    {
        public UnableToDeleteTheatreException()
        {
        }

        public UnableToDeleteTheatreException(string? message) : base(message)
        {
        }

        public UnableToDeleteTheatreException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToDeleteTheatreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}