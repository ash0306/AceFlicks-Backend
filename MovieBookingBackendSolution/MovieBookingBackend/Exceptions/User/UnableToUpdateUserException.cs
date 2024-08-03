using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.User
{
    [Serializable]
    public class UnableToUpdateUserException : Exception
    {
        public UnableToUpdateUserException()
        {
        }

        public UnableToUpdateUserException(string? message) : base(message)
        {
        }

        public UnableToUpdateUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToUpdateUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}