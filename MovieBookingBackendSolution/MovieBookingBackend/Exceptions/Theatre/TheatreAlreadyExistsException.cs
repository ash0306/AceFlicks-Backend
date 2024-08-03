using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Theatre
{
    [Serializable]
    public class TheatreAlreadyExistsException : Exception
    {
        public TheatreAlreadyExistsException()
        {
        }

        public TheatreAlreadyExistsException(string? message) : base(message)
        {
        }

        public TheatreAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TheatreAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}