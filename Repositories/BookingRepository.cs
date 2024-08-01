using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Booking;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingBackend.Repositories
{
    public class BookingRepository : IRepository<int, Booking>
    {
        private readonly MovieBookingContext _context;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="context">DB Context for MovieBooking</param>
        public BookingRepository(MovieBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new Booking
        /// </summary>
        /// <param name="item">Booking to be added</param>
        /// <returns>The newly added booking</returns>
        /// <exception cref="ArgumentNullException">Thrown if the booking to be added is null</exception>
        /// <exception cref="UnableToAddBookingException">Thrown if the booking cannot be added</exception>
        public async Task<Booking> Add(Booking item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.Bookings.Add(item);
            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToAddBookingException($"Unable to add the booking with ID: {item.Id}");
            }
            return item;
        }

        /// <summary>
        /// Deletes a booking
        /// </summary>
        /// <param name="key">ID of the booking to be deleted</param>
        /// <returns>The deleted booking</returns>
        /// <exception cref="NoSuchBookingException">Thrown if no booking with the given ID exists</exception>
        /// <exception cref="UnableToDeleteBookingException">Thrown if the booking cannot be deleted</exception>
        public async Task<Booking> Delete(int key)
        {
            var booking = await GetById(key);
            if (booking == null)
            {
                throw new NoSuchBookingException($"No booking with ID {key} was found");
            }
            _context.Remove(booking);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteBookingException($"Unable to delete booking with ID {key}");
            }

            return booking;
        }

        /// <summary>
        /// Deletes a range of bookings
        /// </summary>
        /// <param name="key">ID of the bookings to be deleted</param>
        /// <returns>Boolean result of the deletion operation</returns>
        /// <exception cref="NoBookingsFoundException">Thrown if no bookings match the ID</exception>
        /// <exception cref="UnableToDeleteBookingException">Thrown if the bookings cannot be deleted</exception>
        public async Task<bool> DeleteRange(IList<int> key)
        {
            IList<Booking> bookings = new List<Booking>();
            foreach (var id in key)
            {
                var result = await GetById(id);
                bookings.Add(result);
            }
            if (bookings.Count() <= 0)
            {
                throw new NoBookingsFoundException("No bookings found for the given constraint");
            }
            _context.RemoveRange(bookings);
            int noOfRowsAffected = await _context.SaveChangesAsync();

            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteBookingException("Unable to delete the set of bookings");
            }
            return true;
        }

        /// <summary>
        /// Gets a list of all bookings
        /// </summary>
        /// <returns>List of all bookings</returns>
        /// <exception cref="NoBookingsFoundException">If no bookings are found</exception>
        public async Task<IEnumerable<Booking>> GetAll()
        {
            var bookings = await _context.Bookings.Include(b => b.Seats).Include(b => b.Showtime.Movie).Include(b => b.Showtime.Theatre).ToListAsync();

            if (bookings.Count() <= 0)
            {
                throw new NoBookingsFoundException("No bookings found");
            }
            return bookings;
        }

        /// <summary>
        /// Gets a booking given its ID
        /// </summary>
        /// <param name="key">ID of the booking to be fetched</param>
        /// <returns>Booking with the given ID</returns>
        /// <exception cref="NoSuchBookingException">If booking with the ID doesn't exist</exception>
        public async Task<Booking> GetById(int key)
        {
            var booking = await _context.Bookings
                .Include(b => b.Seats)
                .Include(b => b.Showtime)
                .FirstOrDefaultAsync(b => b.Id == key);
            if (booking == null)
            {
                throw new NoSuchBookingException($"No booking with ID {key} was found");
            }
            return booking;
        }

        /// <summary>
        /// Updates the booking details
        /// </summary>
        /// <param name="item">Booking with the details to be updated</param>
        /// <returns>Updated booking</returns>
        /// <exception cref="NoSuchBookingException">Thrown if booking with the ID doesn't exist</exception>
        /// <exception cref="UnableToUpdateBookingException">Thrown if the booking cannot be updated</exception>
        public async Task<Booking> Update(Booking item)
        {
            var booking = await GetById(item.Id);
            if (booking == null)
            {
                throw new NoSuchBookingException($"No booking with ID {item.Id} was found");
            }
            _context.Update(item);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToUpdateBookingException($"Unable to update booking with ID {item.Id}");
            }

            return item;
        }
    }
}
