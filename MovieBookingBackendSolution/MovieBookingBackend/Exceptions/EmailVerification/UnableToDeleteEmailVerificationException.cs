using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.EmailVerification
{
    [Serializable]
    public class UnableToDeleteEmailVerificationException : Exception
    {
        public UnableToDeleteEmailVerificationException()
        {
        }

        public UnableToDeleteEmailVerificationException(string? message) : base(message)
        {
        }

        public UnableToDeleteEmailVerificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToDeleteEmailVerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}