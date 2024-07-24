using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Showtime
{
    [Serializable]
    internal class NoShowtimesFoundException : Exception
    {
        public NoShowtimesFoundException()
        {
        }

        public NoShowtimesFoundException(string? message) : base(message)
        {
        }

        public NoShowtimesFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoShowtimesFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}