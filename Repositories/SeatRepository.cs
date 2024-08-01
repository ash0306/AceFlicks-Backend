using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Seat;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingBackend.Repositories
{
    public class SeatRepository : IRepository<int, Seat>
    {
        private readonly MovieBookingContext _context;

        /// <summary>
        /// Parameterised Constructor
        /// </summary>
        /// <param name="context">DB Context for MovieBooking</param>
        public SeatRepository(MovieBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new Seat
        /// </summary>
        /// <param name="item">Seat to be added</param>
        /// <returns>The newly added seat</returns>
        /// <exception cref="ArgumentNullException">Thrown if the seat to be added is null</exception>
        /// <exception cref="UnableToAddSeatException">Thrown if the seat cannot be added</exception>
        public async Task<Seat> Add(Seat item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.Seats.Add(item);
            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToAddSeatException($"Unable to add the seat with ID: {item.Id}");
            }
            return item;
        }

        /// <summary>
        /// Deletes a seat
        /// </summary>
        /// <param name="key">ID of the seat to be deleted</param>
        /// <returns>The deleted seat</returns>
        /// <exception cref="NoSuchSeatException">Thrown if no seat with the given ID exists</exception>
        /// <exception cref="UnableToDeleteSeatException">Thrown if the seat cannot be deleted</exception>
        public async Task<Seat> Delete(int key)
        {
            var seat = await GetById(key);
            if (seat == null)
            {
                throw new NoSuchSeatException($"No seat with ID {key} was found");
            }
            _context.Remove(seat);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteSeatException($"Unable to delete seat with ID {key}");
            }

            return seat;
        }

        /// <summary>
        /// Deletes a range of seats
        /// </summary>
        /// <param name="key">ID of the seats to be deleted</param>
        /// <returns>Boolean result of the deletion operation</returns>
        /// <exception cref="NoSeatsFoundException">Thrown if no seats match the ID</exception>
        /// <exception cref="UnableToDeleteSeatException">Thrown if the seats cannot be deleted</exception>
        public async Task<bool> DeleteRange(IList<int> key)
        {
            IList<Seat> seats = new List<Seat>();
            foreach (var id in key)
            {
                var result = await GetById(id);
                seats.Add(result);
            }
            if (seats.Count() <= 0)
            {
                throw new NoSeatsFoundException("No seats found for the given constraint");
            }
            _context.RemoveRange(seats);
            int noOfRowsAffected = await _context.SaveChangesAsync();

            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteSeatException("Unable to delete the set of seats");
            }
            return true;
        }

        /// <summary>
        /// Gets a list of all seats
        /// </summary>
        /// <returns>List of all seats</returns>
        /// <exception cref="NoSeatsFoundException">If no seats are found</exception>
        public async Task<IEnumerable<Seat>> GetAll()
        {
            var seats = await _context.Seats.ToListAsync();

            if (seats.Count() <= 0)
            {
                throw new NoSeatsFoundException("No seats found");
            }
            return seats;
        }

        /// <summary>
        /// Gets a seat given its ID
        /// </summary>
        /// <param name="key">ID of the seat to be fetched</param>
        /// <returns>Seat with the given ID</returns>
        /// <exception cref="NoSuchSeatException">If seat with the ID doesn't exist</exception>
        public async Task<Seat> GetById(int key)
        {
            var seat = await _context.Seats.FirstOrDefaultAsync(s => s.Id == key);
            if (seat == null)
            {
                throw new NoSuchSeatException($"No seat with ID {key} was found");
            }
            return seat;
        }

        /// <summary>
        /// Updates the seat details
        /// </summary>
        /// <param name="item">Seat with the details to be updated</param>
        /// <returns>Updated seat</returns>
        /// <exception cref="NoSuchSeatException">Thrown if seat with the ID doesn't exist</exception>
        /// <exception cref="UnableToUpdateSeatException">Thrown if the seat cannot be updated</exception>
        public async Task<Seat> Update(Seat item)
        {
            var seat = await GetById(item.Id);
            if (seat == null)
            {
                throw new NoSuchSeatException($"No seat with ID {item.Id} was found");
            }
            _context.Update(item);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToUpdateSeatException($"Unable to update seat with ID {item.Id}");
            }

            return item;
        }
    }
}
