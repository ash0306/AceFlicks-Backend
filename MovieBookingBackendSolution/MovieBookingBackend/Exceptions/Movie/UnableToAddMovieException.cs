using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Movie
{
    [Serializable]
    public class UnableToAddMovieException : Exception
    {
        public UnableToAddMovieException()
        {
        }

        public UnableToAddMovieException(string? message) : base(message)
        {
        }

        public UnableToAddMovieException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToAddMovieException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}