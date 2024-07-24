using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Theatre
{
    [Serializable]
    internal class UnableToAddTheatreException : Exception
    {
        public UnableToAddTheatreException()
        {
        }

        public UnableToAddTheatreException(string? message) : base(message)
        {
        }

        public UnableToAddTheatreException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToAddTheatreException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}