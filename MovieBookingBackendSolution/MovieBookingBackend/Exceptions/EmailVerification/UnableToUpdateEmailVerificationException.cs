using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.EmailVerification
{
    [Serializable]
    public class UnableToUpdateEmailVerificationException : Exception
    {
        public UnableToUpdateEmailVerificationException()
        {
        }

        public UnableToUpdateEmailVerificationException(string? message) : base(message)
        {
        }

        public UnableToUpdateEmailVerificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToUpdateEmailVerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}