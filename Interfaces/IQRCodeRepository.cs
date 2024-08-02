using MovieBookingBackend.Models;

namespace MovieBookingBackend.Interfaces
{
    public interface IQRCodeRepository
    {
        public Task<QRCode> Add(QRCode qrCode);
        public Task<QRCode> GetById(int id);
        public Task<IEnumerable<QRCode>> GetAll();
    }
}
