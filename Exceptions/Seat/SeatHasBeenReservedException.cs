using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Seat
{
    [Serializable]
    internal class SeatHasBeenReservedException : Exception
    {
        public SeatHasBeenReservedException()
        {
        }

        public SeatHasBeenReservedException(string? message) : base(message)
        {
        }

        public SeatHasBeenReservedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SeatHasBeenReservedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}