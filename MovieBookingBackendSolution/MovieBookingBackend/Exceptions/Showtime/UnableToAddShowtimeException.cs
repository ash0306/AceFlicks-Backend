using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Showtime
{
    [Serializable]
    public class UnableToAddShowtimeException : Exception
    {
        public UnableToAddShowtimeException()
        {
        }

        public UnableToAddShowtimeException(string? message) : base(message)
        {
        }

        public UnableToAddShowtimeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToAddShowtimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}