using Microsoft.EntityFrameworkCore;
using MovieBookingBackend.Contexts;
using MovieBookingBackend.Exceptions.EmailVerification;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingBackend.Repositories
{
    public class EmailVerificationRepository : IEmailVerificationRepository
    {
        private readonly MovieBookingContext _context;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="context">DB Context for MovieBooking</param>
        public EmailVerificationRepository(MovieBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new Email Verification
        /// </summary>
        /// <param name="item">Email Verification to be added</param>
        /// <returns>The newly added email verification</returns>
        /// <exception cref="ArgumentNullException">Thrown if the email verification to be added is null</exception>
        /// <exception cref="UnableToAddEmailVerificationException">Thrown if the email verification cannot be added</exception>
        public async Task<EmailVerification> Add(EmailVerification item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _context.EmailVerifications.Add(item);
            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToAddEmailVerificationException($"Unable to add the email verification with ID: {item.Id}");
            }
            return item;
        }

        /// <summary>
        /// Deletes an email verification
        /// </summary>
        /// <param name="key">ID of the email verification to be deleted</param>
        /// <returns>The deleted email verification</returns>
        /// <exception cref="NoSuchEmailVerificationException">Thrown if no email verification with the given ID exists</exception>
        /// <exception cref="UnableToDeleteEmailVerificationException">Thrown if the email verification cannot be deleted</exception>
        public async Task<EmailVerification> Delete(int key)
        {
            var emailVerification = await GetById(key);
            if (emailVerification == null)
            {
                throw new NoSuchEmailVerificationException($"No email verification with ID {key} was found");
            }
            _context.Remove(emailVerification);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteEmailVerificationException($"Unable to delete email verification with ID {key}");
            }

            return emailVerification;
        }

        /// <summary>
        /// Deletes a range of email verifications
        /// </summary>
        /// <param name="key">ID of the email verifications to be deleted</param>
        /// <returns>Boolean result of the deletion operation</returns>
        /// <exception cref="NoEmailVerificationsFoundException">Thrown if no email verifications match the ID</exception>
        /// <exception cref="UnableToDeleteEmailVerificationException">Thrown if the email verifications cannot be deleted</exception>
        public async Task<bool> DeleteRange(IList<int> key)
        {
            IList<EmailVerification> emails = new List<EmailVerification>();
            foreach (var id in key)
            {
                var result = await GetById(id);
                emails.Add(result);
            }
            if (emails.Count() <= 0)
            {
                throw new NoEmailVerificationsFoundException("No email verifications found for the given constraint");
            }
            _context.RemoveRange(emails);
            int noOfRowsAffected = await _context.SaveChangesAsync();

            if (noOfRowsAffected <= 0)
            {
                throw new UnableToDeleteEmailVerificationException("Unable to delete the set of email verifications");
            }
            return true;
        }

        /// <summary>
        /// Gets a list of all email verifications
        /// </summary>
        /// <returns>List of all email verifications</returns>
        /// <exception cref="NoEmailVerificationsFoundException">If no email verifications are found</exception>
        public async Task<IEnumerable<EmailVerification>> GetAll()
        {
            var emailVerifications = await _context.EmailVerifications.Include(ev => ev.User).ToListAsync();

            if (emailVerifications.Count() <= 0)
            {
                throw new NoEmailVerificationsFoundException("No email verifications found");
            }
            return emailVerifications;
        }

        /// <summary>
        /// Gets an email verification given its ID
        /// </summary>
        /// <param name="key">ID of the email verification to be fetched</param>
        /// <returns>Email verification with the given ID</returns>
        /// <exception cref="NoSuchEmailVerificationException">If email verification with the ID doesn't exist</exception>
        public async Task<EmailVerification> GetById(int key)
        {
            var emailVerification = await _context.EmailVerifications
                .Include(ev => ev.User)
                .FirstOrDefaultAsync(ev => ev.Id == key);
            if (emailVerification == null)
            {
                throw new NoSuchEmailVerificationException($"No email verification with ID {key} was found");
            }
            return emailVerification;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<EmailVerification> GetByUserId(int userId)
        {
            var result = await _context.EmailVerifications.FirstOrDefaultAsync(ev => ev.UserId == userId);
            return result;
        }

        /// <summary>
        /// Updates the email verification details
        /// </summary>
        /// <param name="item">Email verification with the details to be updated</param>
        /// <returns>Updated email verification</returns>
        /// <exception cref="NoSuchEmailVerificationException">Thrown if email verification with the ID doesn't exist</exception>
        /// <exception cref="UnableToUpdateEmailVerificationException">Thrown if the email verification cannot be updated</exception>
        public async Task<EmailVerification> Update(EmailVerification item)
        {
            var emailVerification = await GetById(item.Id);
            if (emailVerification == null)
            {
                throw new NoSuchEmailVerificationException($"No email verification with ID {item.Id} was found");
            }
            _context.Update(item);

            int noOfRowsAffected = await _context.SaveChangesAsync();
            if (noOfRowsAffected <= 0)
            {
                throw new UnableToUpdateEmailVerificationException($"Unable to update email verification with ID {item.Id}");
            }

            return item;
        }
    }
}
