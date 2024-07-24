using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.User
{
    [Serializable]
    internal class UnableToDeleteUserException : Exception
    {
        public UnableToDeleteUserException()
        {
        }

        public UnableToDeleteUserException(string? message) : base(message)
        {
        }

        public UnableToDeleteUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToDeleteUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}