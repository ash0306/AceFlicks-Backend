using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.User;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;

namespace MovieBookingBackend.Repositories
{
    public class UserRepository : IRepository<int, User>
    {
        private readonly MovieBookingContext _context;

        /// <summary>
        /// Parameterised Constructor
        /// </summary>
        /// <param name="context">DB Context for MovieBooking</param>
        public UserRepository(MovieBookingContext context) 
        { 
            _context = context;
        }

        /// <summary>
        /// Adds a new User
        /// </summary>
        /// <param name="item">User to be added</param>
        /// <returns>The newly added user</returns>
        /// <exception cref="ArgumentNullException">Thrown if the user to added is null</exception>
        /// <exception cref="UnableToAddUserException">Thrown if user cannot be added</exception>
        public async Task<User> Add(User item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.Users.Add(item);
            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0) 
            {
                throw new UnableToAddUserException($"Unable to add the user with ID: {item.Id}");
            }
            return item;
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="key">ID of the user to be deleted</param>
        /// <returns>The deleted user</returns>
        /// <exception cref="NoSuchUserException">Thrown if no user if the given ID exists</exception>
        /// <exception cref="UnableToDeleteUserException">Thrown if the user cannot be deleted</exception>
        public async Task<User> Delete(int key)
        {
            var user = await GetById(key);
            if (user == null)
            {
                throw new NoSuchUserException($"No user with ID {key} was found");
            }
            _context.Remove(user);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if(noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteUserException($"Unable to delete user with ID {key}");
            }

            return user;
        }

        /// <summary>
        /// Deletes a range of users
        /// </summary>
        /// <param name="key">ID of the users to be deleted</param>
        /// <returns>Boolean result of tthe deletion operation</returns>
        /// <exception cref="NoUsersFoundException">THrown if no users match the ID</exception>
        /// <exception cref="UnableToDeleteUserException">Thrown if the user cannot be deleted</exception>
        public async Task<bool> DeleteRange(int key)
        {
            var users = (await GetAll()).ToList().Where(u => u.Id == key);
            if(users.Count() <= 0)
            {
                throw new NoUsersFoundException("No users found for the given constraint");
            }
            _context.RemoveRange(users);
            int noOfRowsAffected = await _context.SaveChangesAsync();

            if(noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteUserException("Unable to delete the set of users");
            }
            return true;
        }

        /// <summary>
        /// Gets a list of the users
        /// </summary>
        /// <returns>List of all users</returns>
        /// <exception cref="NoUsersFoundException">If no users are found</exception>
        public async Task<IEnumerable<User>> GetAll()
        {
            var users = await _context.Users.ToListAsync();

            if(users.Count() <= 0)
            {
                throw new NoUsersFoundException("No users found");
            }
            return users;
        }

        /// <summary>
        /// Gets a user given their ID
        /// </summary>
        /// <param name="key">ID of the user to be fetched</param>
        /// <returns>User with the given ID</returns>
        /// <exception cref="NoSuchUserException">If user with thte ID doesnt exist</exception>
        public async Task<User> GetById(int key)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == key);
            if(user == null)
            {
                throw new NoSuchUserException($"No user with ID {key} was found");
            }
            return user;
        }

        /// <summary>
        /// Updates the user details
        /// </summary>
        /// <param name="item">User with the details to be updated</param>
        /// <returns>Updated user</returns>
        /// <exception cref="NoSuchUserException">Thrown if user with the ID doent exist</exception>
        /// <exception cref="UnableToUpdateUserException">Thrown if user cannot be updated</exception>
        public async Task<User> Update(User item)
        {
            var user = await GetById(item.Id);
            if (user == null)
            {
                throw new NoSuchUserException($"No user with ID {item.Id} was found");
            }
            _context.Update(item);

            await _context.SaveChangesAsync();

            return user;
        }
    }
}
