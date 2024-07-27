using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Auth
{
    [Serializable]
    internal class UserNotActiveException : Exception
    {
        public UserNotActiveException()
        {
        }

        public UserNotActiveException(string? message) : base(message)
        {
        }

        public UserNotActiveException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UserNotActiveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}