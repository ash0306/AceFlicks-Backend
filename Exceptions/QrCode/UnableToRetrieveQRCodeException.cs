using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.QrCode
{
    [Serializable]
    internal class UnableToRetrieveQRCodeException : Exception
    {
        public UnableToRetrieveQRCodeException()
        {
        }

        public UnableToRetrieveQRCodeException(string? message) : base(message)
        {
        }

        public UnableToRetrieveQRCodeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnableToRetrieveQRCodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}