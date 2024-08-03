using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Seat
{
    [Serializable]
    public class SeatAlreadyExistsException : Exception
    {
        public SeatAlreadyExistsException()
        {
        }

        public SeatAlreadyExistsException(string? message) : base(message)
        {
        }

        public SeatAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SeatAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}