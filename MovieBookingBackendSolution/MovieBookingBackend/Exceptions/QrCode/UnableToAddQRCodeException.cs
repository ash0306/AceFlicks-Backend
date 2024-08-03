using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.QrCode
{
    [Serializable]
    public class UnableToAddQRCodeException : Exception
    {
        public UnableToAddQRCodeException()
        {
        }

        public UnableToAddQRCodeException(string? message) : base(message)
        {
        }

        public UnableToAddQRCodeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToAddQRCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}