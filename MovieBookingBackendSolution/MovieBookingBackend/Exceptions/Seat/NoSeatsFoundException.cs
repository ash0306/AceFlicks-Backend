using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Seat
{
    [Serializable]
    public class NoSeatsFoundException : Exception
    {
        public NoSeatsFoundException()
        {
        }

        public NoSeatsFoundException(string? message) : base(message)
        {
        }

        public NoSeatsFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoSeatsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}