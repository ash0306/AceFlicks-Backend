using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.Email
{
    [Serializable]
    public class UnableToSendMailException : Exception
    {
        public UnableToSendMailException()
        {
        }

        public UnableToSendMailException(string? message) : base(message)
        {
        }

        public UnableToSendMailException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToSendMailException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}