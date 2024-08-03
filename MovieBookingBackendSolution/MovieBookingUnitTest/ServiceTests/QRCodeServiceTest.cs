using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.QrCode;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models.DTOs.Bookings;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using MovieBookingBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTest.ServiceTests
{
    public class QRCodeServiceTest
    {
        private MovieBookingContext _context;
        private IQRCodeRepository _qrCodeRepository;
        private IQRCodeService _qrCodeService;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _qrCodeRepository = new QRCodeRepository(_context);

            _qrCodeService = new QRCodeService(_context, _qrCodeRepository);

            await SeedDatabase();
        }

        private async Task SeedDatabase()
        {
            var booking = new Booking
            {
                Id = 1,
                ShowtimeId = 1,
                UserId = 1
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateQRCode_CreatesQRCodeSuccessfully()
        {
            var bookingDTO = new BookingDTO
            {
                Id = 1,
                ShowtimeDetails = new List<string>
                {
                    "Inception",
                    "Grand Cinema"
                },
                Seats = new[] { "A1", "A2" }
            };

            var qrCodeData = await _qrCodeService.CreateQRCode(bookingDTO);

            Assert.IsNotNull(qrCodeData);
        }

        [Test]
        public async Task GetQRCodeByBookingId_ReturnsQRCodeSuccessfully()
        {
            var bookingDTO = new BookingDTO
            {
                Id = 1,
                ShowtimeDetails = new List<string>
                {
                    "Inception",
                    "Grand Cinema"
                },
                Seats = new[] { "A1", "A2" }
            };
            await _qrCodeService.CreateQRCode(bookingDTO);

            var qrCodeData = await _qrCodeService.GetQRCodeByBookingId(1);

            Assert.IsNotNull(qrCodeData);
        }

        [Test]
        public void GetQRCodeByBookingId_NoQRCodeFound_ThrowsQRCodeNotFoundException()
        {
            Assert.ThrowsAsync<QRCodeNotFoundException>(() => _qrCodeService.GetQRCodeByBookingId(999));
        }
    }
}
