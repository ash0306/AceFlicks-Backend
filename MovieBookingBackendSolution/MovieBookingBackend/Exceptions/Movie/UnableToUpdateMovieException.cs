using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Movie
{
    [Serializable]
    public class UnableToUpdateMovieException : Exception
    {
        public UnableToUpdateMovieException()
        {
        }

        public UnableToUpdateMovieException(string? message) : base(message)
        {
        }

        public UnableToUpdateMovieException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToUpdateMovieException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}