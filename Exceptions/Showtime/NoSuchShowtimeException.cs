using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Showtime
{
    [Serializable]
    internal class NoSuchShowtimeException : Exception
    {
        public NoSuchShowtimeException()
        {
        }

        public NoSuchShowtimeException(string? message) : base(message)
        {
        }

        public NoSuchShowtimeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoSuchShowtimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}