using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Seat
{
    [Serializable]
    public class UnableToAddSeatException : Exception
    {
        public UnableToAddSeatException()
        {
        }

        public UnableToAddSeatException(string? message) : base(message)
        {
        }

        public UnableToAddSeatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToAddSeatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}