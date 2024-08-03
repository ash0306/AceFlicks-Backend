using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Seat
{
    [Serializable]
    public class NoSuchSeatException : Exception
    {
        public NoSuchSeatException()
        {
        }

        public NoSuchSeatException(string? message) : base(message)
        {
        }

        public NoSuchSeatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoSuchSeatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}