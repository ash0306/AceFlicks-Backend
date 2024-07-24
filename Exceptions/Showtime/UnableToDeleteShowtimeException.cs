using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Showtime
{
    [Serializable]
    internal class UnableToDeleteShowtimeException : Exception
    {
        public UnableToDeleteShowtimeException()
        {
        }

        public UnableToDeleteShowtimeException(string? message) : base(message)
        {
        }

        public UnableToDeleteShowtimeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToDeleteShowtimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}