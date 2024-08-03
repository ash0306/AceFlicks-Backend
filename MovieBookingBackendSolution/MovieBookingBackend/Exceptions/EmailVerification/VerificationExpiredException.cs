using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.EmailVerification
{
    [Serializable]
    public class VerificationExpiredException : Exception
    {
        public VerificationExpiredException()
        {
        }

        public VerificationExpiredException(string? message) : base(message)
        {
        }

        public VerificationExpiredException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected VerificationExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}