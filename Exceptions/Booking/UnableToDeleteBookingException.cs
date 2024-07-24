using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Booking
{
    [Serializable]
    internal class UnableToDeleteBookingException : Exception
    {
        public UnableToDeleteBookingException()
        {
        }

        public UnableToDeleteBookingException(string? message) : base(message)
        {
        }

        public UnableToDeleteBookingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToDeleteBookingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}