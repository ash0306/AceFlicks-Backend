using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Showtime;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingBackend.Repositories
{
    public class ShowtimeRepository : IRepository<int, Showtime>
    {
        private readonly MovieBookingContext _context;

        /// <summary>
        /// Parameterised Constructor
        /// </summary>
        /// <param name="context">DB Context for MovieBooking</param>
        public ShowtimeRepository(MovieBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new Showtime
        /// </summary>
        /// <param name="item">Showtime to be added</param>
        /// <returns>The newly added showtime</returns>
        /// <exception cref="ArgumentNullException">Thrown if the showtime to be added is null</exception>
        /// <exception cref="UnableToAddShowtimeException">Thrown if the showtime cannot be added</exception>
        public async Task<Showtime> Add(Showtime item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.Showtimes.Add(item);
            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToAddShowtimeException($"Unable to add the showtime with ID: {item.Id}");
            }
            var result = await GetById(item.Id);
            return result;
        }

        /// <summary>
        /// Deletes a showtime
        /// </summary>
        /// <param name="key">ID of the showtime to be deleted</param>
        /// <returns>The deleted showtime</returns>
        /// <exception cref="NoSuchShowtimeException">Thrown if no showtime with the given ID exists</exception>
        /// <exception cref="UnableToDeleteShowtimeException">Thrown if the showtime cannot be deleted</exception>
        public async Task<Showtime> Delete(int key)
        {
            var showtime = await GetById(key);
            if (showtime == null)
            {
                throw new NoSuchShowtimeException($"No showtime with ID {key} was found");
            }
            _context.Remove(showtime);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteShowtimeException($"Unable to delete showtime with ID {key}");
            }

            return showtime;
        }

        /// <summary>
        /// Deletes a range of showtimes
        /// </summary>
        /// <param name="key">ID of the showtimes to be deleted</param>
        /// <returns>Boolean result of the deletion operation</returns>
        /// <exception cref="NoShowtimesFoundException">Thrown if no showtimes match the ID</exception>
        /// <exception cref="UnableToDeleteShowtimeException">Thrown if the showtimes cannot be deleted</exception>
        public async Task<bool> DeleteRange(IList<int> key)
        {
            IList<Showtime> showtimes = new List<Showtime>();
            foreach (var id in key)
            {
                var result = await GetById(id);
                showtimes.Add(result);
            }
            if (showtimes.Count() <= 0)
            {
                throw new NoShowtimesFoundException("No showtimes found for the given constraint");
            }
            _context.RemoveRange(showtimes);
            int noOfRowsAffected = await _context.SaveChangesAsync();

            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteShowtimeException("Unable to delete the set of showtimes");
            }
            return true;
        }

        /// <summary>
        /// Gets a list of all showtimes
        /// </summary>
        /// <returns>List of all showtimes</returns>
        /// <exception cref="NoShowtimesFoundException">If no showtimes are found</exception>
        public async Task<IEnumerable<Showtime>> GetAll()
        {
            var showtimes = await _context.Showtimes.Include(s => s.Movie).Include(s => s.Theatre).Include(s => s.Seats).ToListAsync();

            if (showtimes.Count() <= 0)
            {
                throw new NoShowtimesFoundException("No showtimes found");
            }
            return showtimes;
        }

        /// <summary>
        /// Gets a showtime given its ID
        /// </summary>
        /// <param name="key">ID of the showtime to be fetched</param>
        /// <returns>Showtime with the given ID</returns>
        /// <exception cref="NoSuchShowtimeException">If showtime with the ID doesn't exist</exception>
        public async Task<Showtime> GetById(int key)
        {
            var showtime = await _context.Showtimes.Include(s => s.Movie).Include(s => s.Theatre).Include(s => s.Seats).FirstOrDefaultAsync(s => s.Id == key);
            if (showtime == null)
            {
                throw new NoSuchShowtimeException($"No showtime with ID {key} was found");
            }
            return showtime;
        }

        /// <summary>
        /// Updates the showtime details
        /// </summary>
        /// <param name="item">Showtime with the details to be updated</param>
        /// <returns>Updated showtime</returns>
        /// <exception cref="NoSuchShowtimeException">Thrown if showtime with the ID doesn't exist</exception>
        /// <exception cref="UnableToUpdateShowtimeException">Thrown if the showtime cannot be updated</exception>
        public async Task<Showtime> Update(Showtime item)
        {
            var showtime = await GetById(item.Id);
            if (showtime == null)
            {
                throw new NoSuchShowtimeException($"No showtime with ID {item.Id} was found");
            }
            _context.Update(item);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToUpdateShowtimeException($"Unable to update showtime with ID {item.Id}");
            }

            return item;
        }
    }
}
