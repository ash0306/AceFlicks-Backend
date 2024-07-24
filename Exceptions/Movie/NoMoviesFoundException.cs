using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Movie
{
    [Serializable]
    internal class NoMoviesFoundException : Exception
    {
        public NoMoviesFoundException()
        {
        }

        public NoMoviesFoundException(string? message) : base(message)
        {
        }

        public NoMoviesFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoMoviesFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}