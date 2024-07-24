using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Booking
{
    [Serializable]
    internal class NoBookingsFoundException : Exception
    {
        public NoBookingsFoundException()
        {
        }

        public NoBookingsFoundException(string? message) : base(message)
        {
        }

        public NoBookingsFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoBookingsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}