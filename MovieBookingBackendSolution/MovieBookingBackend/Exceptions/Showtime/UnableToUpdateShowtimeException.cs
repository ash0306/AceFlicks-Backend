using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Showtime
{
    [Serializable]
    public class UnableToUpdateShowtimeException : Exception
    {
        public UnableToUpdateShowtimeException()
        {
        }

        public UnableToUpdateShowtimeException(string? message) : base(message)
        {
        }

        public UnableToUpdateShowtimeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToUpdateShowtimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}