using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.User
{
    [Serializable]
    public class UnableToAddUserException : Exception
    {
        public UnableToAddUserException()
        {
        }

        public UnableToAddUserException(string? message) : base(message)
        {
        }

        public UnableToAddUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToAddUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}