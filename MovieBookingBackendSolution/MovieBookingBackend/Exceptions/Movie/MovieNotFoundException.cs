using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Movie
{
    [Serializable]
    public class MovieNotFoundException : Exception
    {
        public MovieNotFoundException()
        {
        }

        public MovieNotFoundException(string? message) : base(message)
        {
        }

        public MovieNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MovieNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}