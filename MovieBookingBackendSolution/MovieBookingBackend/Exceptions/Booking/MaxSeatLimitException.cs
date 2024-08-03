using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Booking
{
    [Serializable]
    public class MaxSeatLimitException : Exception
    {
        public MaxSeatLimitException()
        {
        }

        public MaxSeatLimitException(string? message) : base(message)
        {
        }

        public MaxSeatLimitException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MaxSeatLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}