using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Seat
{
    [Serializable]
    public class UnableToDeleteSeatException : Exception
    {
        public UnableToDeleteSeatException()
        {
        }

        public UnableToDeleteSeatException(string? message) : base(message)
        {
        }

        public UnableToDeleteSeatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToDeleteSeatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}