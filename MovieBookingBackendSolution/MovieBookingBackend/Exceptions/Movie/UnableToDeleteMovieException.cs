using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Movie
{
    [Serializable]
    public class UnableToDeleteMovieException : Exception
    {
        public UnableToDeleteMovieException()
        {
        }

        public UnableToDeleteMovieException(string? message) : base(message)
        {
        }

        public UnableToDeleteMovieException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToDeleteMovieException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}