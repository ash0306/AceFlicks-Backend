using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.QrCode;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Models;
using MovieBookingBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBookingUnitTest.RepositoryTests
{
    public class QRRepositoryTest
    {
        private MovieBookingContext _context;
        private QRCodeRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieBookingContext>()
                .UseInMemoryDatabase(databaseName: "TestQRCodeDatabase")
                .Options;

            _context = new MovieBookingContext(options);
            _repository = new QRCodeRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var booking = new Booking
            {
                UserId = 1,
                ShowtimeId = 1,
                Status = BookingStatus.Booked
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            var qrCodes = new List<QRCode>
            {
                new QRCode
                {
                    BookingId = booking.Id,
                    BookingQR = new byte[] { 0x01, 0x02, 0x03 }
                }
            };

            _context.QRCodes.AddRange(qrCodes);
            _context.SaveChanges();
        }

        [Test, Order(1)]
        public async Task AddQRCode_Success()
        {
            var qrCode = new QRCode
            {
                BookingId = _context.Bookings.First().Id,
                BookingQR = new byte[] { 0x01, 0x02, 0x03, 0x04 }
            };

            var addedQRCode = await _repository.Add(qrCode);

            Assert.NotNull(addedQRCode);
        }

        [Test, Order(2)]
        public void AddQRCode_Failure_NullQRCode()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.Add(null));
        }

        [Test, Order(3)]
        public async Task GetAllQRCodes_Success()
        {
            var qrCodes = await _repository.GetAll();
            Assert.AreEqual(1, qrCodes.Count());
        }

        [Test, Order(4)]
        public async Task GetQRCodeById_Success()
        {
            var qrCode = await _repository.GetById(1);

            Assert.NotNull(qrCode);
        }

        [Test, Order(5)]
        public void GetQRCodeById_Failure_NotFound()
        {
            Assert.ThrowsAsync<UnableToRetrieveQRCodeException>(async () => await _repository.GetById(-1));
        }

        [Test, Order(6)]
        public async Task GetQRCodeById_Failure_UnableToRetrieve()
        {
            // Here we are making the context null to simulate a failure scenario.
            _context.Dispose();
            _context = null;

            Assert.ThrowsAsync<UnableToRetrieveQRCodeException>(async () => await _repository.GetById(1));
        }
    }
}
