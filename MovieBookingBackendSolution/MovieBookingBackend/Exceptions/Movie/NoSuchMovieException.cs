using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Movie
{
    [Serializable]
    public class NoSuchMovieException : Exception
    {
        public NoSuchMovieException()
        {
        }

        public NoSuchMovieException(string? message) : base(message)
        {
        }

        public NoSuchMovieException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoSuchMovieException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}