using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Movie
{
    [Serializable]
    internal class MovieAlreadyExistsException : Exception
    {
        public MovieAlreadyExistsException()
        {
        }

        public MovieAlreadyExistsException(string? message) : base(message)
        {
        }

        public MovieAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MovieAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}