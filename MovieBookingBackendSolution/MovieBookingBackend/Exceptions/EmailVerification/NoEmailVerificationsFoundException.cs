using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.EmailVerification
{
    [Serializable]
    public class NoEmailVerificationsFoundException : Exception
    {
        public NoEmailVerificationsFoundException()
        {
        }

        public NoEmailVerificationsFoundException(string? message) : base(message)
        {
        }

        public NoEmailVerificationsFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoEmailVerificationsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}