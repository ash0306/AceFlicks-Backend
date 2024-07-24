using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Booking
{
    [Serializable]
    internal class UnableToUpdateBookingException : Exception
    {
        public UnableToUpdateBookingException()
        {
        }

        public UnableToUpdateBookingException(string? message) : base(message)
        {
        }

        public UnableToUpdateBookingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToUpdateBookingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}