using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Seat
{
    [Serializable]
    public class UnableToUpdateSeatException : Exception
    {
        public UnableToUpdateSeatException()
        {
        }

        public UnableToUpdateSeatException(string? message) : base(message)
        {
        }

        public UnableToUpdateSeatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToUpdateSeatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}