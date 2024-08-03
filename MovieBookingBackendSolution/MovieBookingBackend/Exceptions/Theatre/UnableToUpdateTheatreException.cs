using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Theatre
{
    [Serializable]
    public class UnableToUpdateTheatreException : Exception
    {
        public UnableToUpdateTheatreException()
        {
        }

        public UnableToUpdateTheatreException(string? message) : base(message)
        {
        }

        public UnableToUpdateTheatreException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToUpdateTheatreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}