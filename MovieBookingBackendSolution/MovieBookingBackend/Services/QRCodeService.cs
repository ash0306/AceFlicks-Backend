using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Bookings;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using MovieBookingBackend.Exceptions.QrCode;

namespace MovieBookingBackend.Services
{
    public class QRCodeService : IQRCodeService
    {
        private readonly MovieBookingContext _context;
        private readonly IQRCodeRepository _qrCodeRepository;

        public QRCodeService(MovieBookingContext context, IQRCodeRepository qrCodeRepository)
        {
            _context = context;
            _qrCodeRepository = qrCodeRepository;
        }

        public async Task<byte[]> CreateQRCode(BookingDTO bookingDTO)
        {
            // Prepare data for QR code
            var qrData = new StringBuilder();
            qrData.AppendLine($"Movie: {string.Join(", ", bookingDTO.ShowtimeDetails)}");
            qrData.AppendLine($"Seats: {string.Join(", ", bookingDTO.Seats)}");

            var qrCodeData = GenerateQRCode(qrData.ToString());
            var qrCode = new QRCode
            {
                BookingId = bookingDTO.Id,
                BookingQR = qrCodeData
            };

            await _qrCodeRepository.Add(qrCode);

            return qrCodeData;
        }

        public async Task<byte[]> GetQRCodeByBookingId(int bookingId)
        {
            var qrCode = (await _qrCodeRepository.GetAll()).FirstOrDefault(q => q.BookingId == bookingId);
            if (qrCode == null)
            {
                throw new QRCodeNotFoundException("QR Code not found for this booking");
            }
            return qrCode.BookingQR;
        }

        private byte[] GenerateQRCode(string data)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new BitmapByteQRCode(qrCodeData))
                {
                    return qrCode.GetGraphic(20); // Generate the QR code image as a byte array
                }
            }
        }
    }
}
