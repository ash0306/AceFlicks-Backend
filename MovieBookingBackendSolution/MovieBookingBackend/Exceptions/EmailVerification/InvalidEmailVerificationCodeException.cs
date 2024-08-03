using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.EmailVerification
{
    [Serializable]
    public class InvalidEmailVerificationCodeException : Exception
    {
        public InvalidEmailVerificationCodeException()
        {
        }

        public InvalidEmailVerificationCodeException(string? message) : base(message)
        {
        }

        public InvalidEmailVerificationCodeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidEmailVerificationCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}