using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Booking
{
    [Serializable]
    public class NoSuchBookingException : Exception
    {
        public NoSuchBookingException()
        {
        }

        public NoSuchBookingException(string? message) : base(message)
        {
        }

        public NoSuchBookingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoSuchBookingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}