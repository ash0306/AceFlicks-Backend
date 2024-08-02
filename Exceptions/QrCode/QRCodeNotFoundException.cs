using System.Runtime.Serialization;

namespace MovieBookingBackend.Exceptions.QrCode
{
    [Serializable]
    internal class QRCodeNotFoundException : Exception
    {
        public QRCodeNotFoundException()
        {
        }

        public QRCodeNotFoundException(string? message) : base(message)
        {
        }

        public QRCodeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected QRCodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}