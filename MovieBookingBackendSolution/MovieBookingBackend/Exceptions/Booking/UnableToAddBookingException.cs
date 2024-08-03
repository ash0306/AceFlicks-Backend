using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Booking
{
    [Serializable]
    public class UnableToAddBookingException : Exception
    {
        public UnableToAddBookingException()
        {
        }

        public UnableToAddBookingException(string? message) : base(message)
        {
        }

        public UnableToAddBookingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToAddBookingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}