using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.EmailVerification
{
    [Serializable]
    internal class UnableToAddEmailVerificationException : Exception
    {
        public UnableToAddEmailVerificationException()
        {
        }

        public UnableToAddEmailVerificationException(string? message) : base(message)
        {
        }

        public UnableToAddEmailVerificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToAddEmailVerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}