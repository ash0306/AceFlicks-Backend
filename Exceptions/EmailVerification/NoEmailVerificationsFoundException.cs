using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.EmailVerification
{
    [Serializable]
    internal class NoEmailVerificationsFoundException : Exception
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