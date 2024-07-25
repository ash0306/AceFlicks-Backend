using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.Theatre;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingBackend.Repositories
{
    public class TheatreRepository : IRepository<int, Theatre>
    {
        private readonly MovieBookingContext _context;

        /// <summary>
        /// Parameterised Constructor
        /// </summary>
        /// <param name="context">DB Context for MovieBooking</param>
        public TheatreRepository(MovieBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new Theatre
        /// </summary>
        /// <param name="item">Theatre to be added</param>
        /// <returns>The newly added theatre</returns>
        /// <exception cref="ArgumentNullException">Thrown if the theatre to be added is null</exception>
        /// <exception cref="UnableToAddTheatreException">Thrown if the theatre cannot be added</exception>
        public async Task<Theatre> Add(Theatre item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.Theatres.Add(item);
            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToAddTheatreException($"Unable to add the theatre with ID: {item.Id}");
            }
            return item;
        }

        /// <summary>
        /// Deletes a theatre
        /// </summary>
        /// <param name="key">ID of the theatre to be deleted</param>
        /// <returns>The deleted theatre</returns>
        /// <exception cref="NoSuchTheatreException">Thrown if no theatre with the given ID exists</exception>
        /// <exception cref="UnableToDeleteTheatreException">Thrown if the theatre cannot be deleted</exception>
        public async Task<Theatre> Delete(int key)
        {
            var theatre = await GetById(key);
            if (theatre == null)
            {
                throw new NoSuchTheatreException($"No theatre with ID {key} was found");
            }
            _context.Remove(theatre);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteTheatreException($"Unable to delete theatre with ID {key}");
            }

            return theatre;
        }

        /// <summary>
        /// Deletes a range of theatres
        /// </summary>
        /// <param name="key">ID of the theatres to be deleted</param>
        /// <returns>Boolean result of the deletion operation</returns>
        /// <exception cref="NoTheatresFoundException">Thrown if no theatres match the ID</exception>
        /// <exception cref="UnableToDeleteTheatreException">Thrown if the theatres cannot be deleted</exception>
        public async Task<bool> DeleteRange(int key)
        {
            var theatres = (await GetAll()).ToList().Where(t => t.Id == key);
            if (theatres.Count() <= 0)
            {
                throw new NoTheatresFoundException("No theatres found for the given constraint");
            }
            _context.RemoveRange(theatres);
            int noOfRowsAffected = await _context.SaveChangesAsync();

            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteTheatreException("Unable to delete the set of theatres");
            }
            return true;
        }

        /// <summary>
        /// Gets a list of all theatres
        /// </summary>
        /// <returns>List of all theatres</returns>
        /// <exception cref="NoTheatresFoundException">If no theatres are found</exception>
        public async Task<IEnumerable<Theatre>> GetAll()
        {
            var theatres = await _context.Theatres.Include(t => t.Showtimes).ToListAsync();

            if (theatres.Count() <= 0)
            {
                throw new NoTheatresFoundException("No theatres found");
            }
            return theatres;
        }

        /// <summary>
        /// Gets a theatre given its ID
        /// </summary>
        /// <param name="key">ID of the theatre to be fetched</param>
        /// <returns>Theatre with the given ID</returns>
        /// <exception cref="NoSuchTheatreException">If theatre with the ID doesn't exist</exception>
        public async Task<Theatre> GetById(int key)
        {
            var theatre = await _context.Theatres.Include(t => t.Showtimes).FirstOrDefaultAsync(t => t.Id == key);
            if (theatre == null)
            {
                throw new NoSuchTheatreException($"No theatre with ID {key} was found");
            }
            return theatre;
        }

        /// <summary>
        /// Updates the theatre details
        /// </summary>
        /// <param name="item">Theatre with the details to be updated</param>
        /// <returns>Updated theatre</returns>
        /// <exception cref="NoSuchTheatreException">Thrown if theatre with the ID doesn't exist</exception>
        /// <exception cref="UnableToUpdateTheatreException">Thrown if the theatre cannot be updated</exception>
        public async Task<Theatre> Update(Theatre item)
        {
            var theatre = await GetById(item.Id);
            if (theatre == null)
            {
                throw new NoSuchTheatreException($"No theatre with ID {item.Id} was found");
            }
            _context.Update(item);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToUpdateTheatreException($"Unable to update theatre with ID {item.Id}");
            }

            return item;
        }
    }
}
