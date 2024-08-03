using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.EmailVerification
{
    [Serializable]
    public class NoSuchEmailVerificationException : Exception
    {
        public NoSuchEmailVerificationException()
        {
        }

        public NoSuchEmailVerificationException(string? message) : base(message)
        {
        }

        public NoSuchEmailVerificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NoSuchEmailVerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}