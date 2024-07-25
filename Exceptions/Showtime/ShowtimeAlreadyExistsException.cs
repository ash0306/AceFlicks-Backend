using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Showtime
{
    [Serializable]
    internal class ShowtimeAlreadyExistsException : Exception
    {
        public ShowtimeAlreadyExistsException()
        {
        }

        public ShowtimeAlreadyExistsException(string? message) : base(message)
        {
        }

        public ShowtimeAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ShowtimeAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}