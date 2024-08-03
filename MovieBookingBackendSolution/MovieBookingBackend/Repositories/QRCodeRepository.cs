using MovieBookingBackend.Contexts;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MovieBookingBackend.Exceptions.QrCode;

namespace MovieBookingBackend.Repositories
{
    public class QRCodeRepository : IQRCodeRepository
    {
        private readonly MovieBookingContext _context;

        public QRCodeRepository(MovieBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new QRCode to the database.
        /// </summary>
        /// <param name="qrCode">The QRCode object to add.</param>
        /// <returns>The added QRCode object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided QRCode object is null.</exception>
        /// <exception cref="UnableToAddQRCodeException">Thrown when the QRCode could not be added to the database.</exception>
        public async Task<QRCode> Add(QRCode qrCode)
        {
            if (qrCode == null)
            {
                throw new ArgumentNullException(nameof(qrCode), "QRCode cannot be null");
            }

            try
            {
                _context.QRCodes.Add(qrCode);
                int affectedRows = await _context.SaveChangesAsync();

                if (affectedRows == 0)
                {
                    throw new UnableToAddQRCodeException("Failed to add QRCode to the database.");
                }

                return qrCode;
            }
            catch (Exception ex)
            {
                throw new UnableToAddQRCodeException($"Error adding QRCode: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all QRCodes from the database.
        /// </summary>
        /// <returns>A list of QRCode objects.</returns>
        /// <exception cref="UnableToRetrieveQRCodeException">Thrown when QRCodes could not be retrieved from the database.</exception>
        public async Task<IEnumerable<QRCode>> GetAll()
        {
            try
            {
                return await _context.QRCodes.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new UnableToRetrieveQRCodeException($"Error retrieving QRCodes: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a QRCode by its ID.
        /// </summary>
        /// <param name="id">The ID of the QRCode to retrieve.</param>
        /// <returns>The QRCode object with the specified ID.</returns>
        /// <exception cref="QRCodeNotFoundException">Thrown when no QRCode with the specified ID is found.</exception>
        /// <exception cref="UnableToRetrieveQRCodeException">Thrown when the QRCode could not be retrieved from the database.</exception>
        public async Task<QRCode> GetById(int id)
        {
            try
            {
                var qrCode = await _context.QRCodes.FirstOrDefaultAsync(q => q.Id == id);
                if (qrCode == null)
                {
                    throw new QRCodeNotFoundException($"No QRCode found with ID {id}");
                }

                return qrCode;
            }
            catch (Exception ex)
            {
                throw new UnableToRetrieveQRCodeException($"Error retrieving QRCode: {ex.Message}");
            }
        }
    }
}
