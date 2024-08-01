using AutoMapper;
using MovieBookingBackend.Exceptions.EmailVerification;
using MovieBookingBackend.Exceptions.User;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Bookings;
using MovieBookingBackend.Models.Enums;
using MovieBookingBackend.Repositories;

namespace MovieBookingBackend.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailService;
        private readonly IRepository<int, User> _userRepository;
        private readonly ILogger<EmailVerificationService> _logger;
        private readonly IShowtimeService _showtimeService;

        /// <summary>
        /// Parameterized Constructor
        /// </summary>
        /// <param name="emailVerificationRepository">Repository for Email Verification</param>
        /// <param name="mapper">Mapper for DTOs</param>
        /// <param name="configuration">Configuration for settings</param>
        /// <param name="emailService">Service for sending emails</param>
        /// <param name="userRepository">Repository for User</param>
        /// <param name="logger">Logger for EmailVerificationService</param>
        public EmailVerificationService(IEmailVerificationRepository emailVerificationRepository, IShowtimeService showtimeService , IMapper mapper, IConfiguration configuration, IEmailSender emailService, IRepository<int, User> userRepository, ILogger<EmailVerificationService> logger)
        {
            _emailVerificationRepository = emailVerificationRepository;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _userRepository = userRepository;
            _logger = logger;
            _showtimeService = showtimeService;
        }

        /// <summary>
        /// Creates a new email verification entry and sends the verification code via email
        /// </summary>
        /// <param name="userId">User ID for which the verification is created</param>
        /// <exception cref="Exception">Thrown when there is an error creating the verification entry</exception>
        public async Task CreateEmailVerification(int userId)
        {
            try
            {
                var exisitingEmailVerification = await _emailVerificationRepository.GetByUserId(userId);
                if (exisitingEmailVerification != null)
                {
                    await _emailVerificationRepository.Delete(exisitingEmailVerification.Id);
                }

                var user = await _userRepository.GetById(userId);
                var verificationCode = GenerateVerificationCode();
                var emailVerification = new EmailVerification
                {
                    UserId = userId,
                    VerificationCode = verificationCode,
                    ExpiryDate = DateTime.Now.AddMinutes(30)
                };

                await _emailVerificationRepository.Add(emailVerification);
                await _emailService.SendEmailAsync(user.Email,
                    $"AceTickets - Login Verification Code {verificationCode}",
                    $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <title>AceTickets Login Verification</title>
                            <style>
                                body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; color: #333; margin: 0; padding: 0; }}
                                .container {{ max-width: 600px; margin: 50px auto; background-color: #fff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); overflow: hidden; }}
                                .header {{ background-color: #FF5733; color: white; padding: 20px; text-align: center; }}
                                .content {{ padding: 30px; }}
                                .footer {{ padding: 20px; font-size: 12px; color: #777; text-align: center; background-color: #f9f9f9; }}
                                .verification-code {{ font-size: 24px; font-weight: bold; margin: 20px 0; }}
                                .info-text {{ color: #555; }}
                                .social-links a {{ text-decoration: none; color: #FF5733; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>AceTickets Login Verification</h1>
                                </div>
                                <div class='content'>
                                    <p>Hello {user.Name},</p>
                                    <p>Welcome to AceTickets! To ensure the security of your account, we need to verify your identity.</p>
                                    <p>Please enter the following code to complete your login:</p>
                                    <div class='verification-code'>{verificationCode}</div>
                                    <p>This code will expire in 30 minutes. If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake.</p>
                                    <p class='info-text'>For your security, never share your verification code with anyone. If you encounter any issues, you can request a new code.</p>
                                </div>
                                <div class='footer'>
                                    <p>Thanks for being part of the AceTickets community!</p>
                                    <p class='social-links'>
                                        Follow us: 
                                        <a href='https://twitter.com/acetickets'>Twitter</a> | 
                                        <a href='https://facebook.com/acetickets'>Facebook</a> | 
                                        <a href='https://instagram.com/acetickets'>Instagram</a>
                                    </p>
                                    <p>© 2024 AceTickets, All Rights Reserved</p>
                                    <p>123 East Street, MarineFord, Chennai, India</p>
                                </div>
                            </div>
                        </body>
                        </html>"
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Verifies the email using the provided verification code
        /// </summary>
        /// <param name="userId">User ID to be verified</param>
        /// <param name="verificationCode">Verification code to verify the email</param>
        /// <returns>True if the verification is successful</returns>
        /// <exception cref="NoSuchEmailVerificationException">Thrown if the email verification is not found</exception>
        /// <exception cref="InvalidEmailVerificationCodeException">Thrown if the verification code is invalid</exception>
        /// <exception cref="VerificationExpiredException">Thrown if the verification code has expired</exception>
        public async Task<bool> VerifyEmail(int userId, string verificationCode)
        {
            var user = await _userRepository.GetById(userId);
            var emailVerification = await _emailVerificationRepository.GetByUserId(userId);
            if (emailVerification == null)
                throw new NoSuchEmailVerificationException("Email verification not found");

            if (emailVerification.VerificationCode != verificationCode)
                throw new InvalidEmailVerificationCodeException("Invalid verification code");

            if (emailVerification.ExpiryDate < DateTime.Now)
            {
                await _emailVerificationRepository.Delete(emailVerification.UserId);
                throw new VerificationExpiredException("Verification code expired");
            }

            await _emailVerificationRepository.Delete(emailVerification.Id);

            if (user.Status != UserStatus.Active)
            {
                user.Status = UserStatus.Active;
                await _userRepository.Update(user);
            }

            await SendVerificationSuccessEmailAsync(userId);

            return true;
        }

        /// <summary>
        /// Sends an email indicating that the email verification was successful
        /// </summary>
        /// <param name="userId">User ID to send the email to</param>
        /// <returns>Task representing the asynchronous operation</returns>
        /// <exception cref="NoSuchUserException">Thrown if the user does not exist</exception>
        private async Task SendVerificationSuccessEmailAsync(int userId)
        {
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                throw new NoSuchUserException($"User with ID {userId} does not exist.");
            }

            var subject = "Email Verification Successful";
            var body = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <title>Email Verification Successful</title>
                            <style>
                                body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; color: #333; margin: 0; padding: 0; }}
                                .container {{ max-width: 600px; margin: 50px auto; background-color: #fff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); overflow: hidden; }}
                                .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; }}
                                .content {{ padding: 30px; }}
                                .footer {{ padding: 20px; font-size: 12px; color: #777; text-align: center; background-color: #f9f9f9; }}
                                .banner {{ font-size: 24px; font-weight: bold; margin: 20px 0; color: #28a745; }}
                                .info-text {{ color: #555; }}
                                .social-links a {{ text-decoration: none; color: #28a745; }}
                                .cta-button {{ display: inline-block; padding: 10px 20px; font-size: 16px; color: #fff; background-color: #28a745; text-decoration: none; border-radius: 5px; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>Email Verification Successful</h1>
                                </div>
                                <div class='content'>
                                    <p>Hello {user.Name},</p>
                                    <p>Your email has been successfully verified! 🎉</p>
                                    <div class='banner'>Enjoy AceTickets to the fullest!</div>
                                </div>
                                <div class='footer'>
                                    <p>Thank you for being a valued member of the AceTickets community!</p>
                                    <p class='social-links'>
                                        Follow us: 
                                        <a href='https://twitter.com/acetickets'>Twitter</a> | 
                                        <a href='https://facebook.com/acetickets'>Facebook</a> | 
                                        <a href='https://instagram.com/acetickets'>Instagram</a>
                                    </p>
                                    <p>© 2024 VibeVault, All Rights Reserved</p>
                                    <p>123 Music Street, Suite 400, Chennai, India</p>
                                </div>
                            </div>
                        </body>
                        </html>";

            await _emailService.SendEmailAsync(user.Email, subject, body);
        }

        /// <summary>
        /// Generates a 6-digit verification code
        /// </summary>
        /// <returns>A 6-digit verification code as string</returns>
        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        /// <summary>
        /// Sends an offer code email to the user
        /// </summary>
        /// <param name="userId">User ID to whom the offer code is sent</param>
        /// <param name="offerCode">The offer code to be sent</param>
        /// <exception cref="Exception">Thrown when there is an error sending the offer code email</exception>
        public async Task SendOfferCodeEmail(int userId, string offerCode)
        {
            try
            {
                var user = await _userRepository.GetById(userId);

                await _emailService.SendEmailAsync(user.Email,
                    $"AceTickets - Special Offer Code Just for You!",
                    $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>AceTickets Special Offer</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; color: #333; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 50px auto; background-color: #fff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); overflow: hidden; }}
                        .header {{ background-color: #FF5733; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 30px; }}
                        .footer {{ padding: 20px; font-size: 12px; color: #777; text-align: center; background-color: #f9f9f9; }}
                        .offer-code {{ font-size: 24px; font-weight: bold; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>AceTickets Special Offer</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {user.Name},</p>
                            <p>We have a special offer just for you! Use the following code to get a free popcorn:</p>
                            <div class='offer-code'>{offerCode}</div>
                            <p>This code is valid for a limited time only. Don't miss out!</p>
                            <p>Avail this offer at the theatre by providing the offer code at the time of billing</p>
                            <p class='info-text'>For your security, never share your offer code with anyone. If you encounter any issues, please contact us.</p>
                        </div>
                        <div class='footer'>
                            <p>Thanks for being part of the AceTickets community!</p>
                            <p class='social-links'>
                                Follow us: 
                                <a href='https://twitter.com/acetickets'>Twitter</a> | 
                                <a href='https://facebook.com/acetickets'>Facebook</a> | 
                                <a href='https://instagram.com/acetickets'>Instagram</a>
                            </p>
                            <p>© 2024 AceTickets, All Rights Reserved</p>
                            <p>123 East Street, MarineFord, Chennai, India</p>
                        </div>
                    </div>
                </body>
                </html>"
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Sends a booking confirmation email to the user
        /// </summary>
        /// <param name="userId">User ID of the person who made the booking</param>
        /// <param name="bookingId">Booking ID for which the confirmation is sent</param>
        /// <exception cref="Exception">Thrown when there is an error sending the booking confirmation email</exception>
        public async Task SendBookingConfirmationEmail(int userId, BookingDTO bookingDTO)
        {
            try
            {
                var user = await _userRepository.GetById(userId);
                var showtimeDetails = (await _showtimeService.GetAllShowtime()).FirstOrDefault(s => s.Id == bookingDTO.ShowtimeId);

                await _emailService.SendEmailAsync(user.Email,
                    $"AceTickets - Booking Confirmation #{bookingDTO.Id}",
                    $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>AceTickets Booking Confirmation</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; color: #333; margin: 0; padding: 0; }}
                        .container {{ max-width: 600px; margin: 50px auto; background-color: #fff; border-radius: 8px; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); overflow: hidden; }}
                        .header {{ background-color: #FF5733; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 30px; }}
                        .footer {{ padding: 20px; font-size: 12px; color: #777; text-align: center; background-color: #f9f9f9; }}
                        .booking-details {{ font-size: 18px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>AceTickets Booking Confirmation</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {user.Name},</p>
                            <p>Thank you for your booking! Here are your booking details:</p>
                            <div class='booking-details'>
                                <p><strong>Booking ID:</strong> {bookingDTO.Id}</p>
                                <p><strong>Movie:</strong> {showtimeDetails.Movie}</p>
                                <p><strong>Theatre:</strong> {showtimeDetails.Theatre} - {showtimeDetails.TheatreLocation}</p>
                                <p><strong>Showtime:</strong> {showtimeDetails.StartTime} to {showtimeDetails.EndTime}</p>
                                <p><strong>Seats:</strong> {string.Join(", ", bookingDTO.Seats)}</p>
                            </div>
                            <p>Enjoy your movie! If you have any questions, feel free to contact us.</p>
                        </div>
                        <div class='footer'>
                            <p>Thanks for choosing AceTickets!</p>
                            <p class='social-links'>
                                Follow us: 
                                <a href='https://twitter.com/acetickets'>Twitter</a> | 
                                <a href='https://facebook.com/acetickets'>Facebook</a> | 
                                <a href='https://instagram.com/acetickets'>Instagram</a>
                            </p>
                            <p>© 2024 AceTickets, All Rights Reserved</p>
                            <p>123 East Street, MarineFord, Chennai, India</p>
                        </div>
                    </div>
                </body>
                </html>"
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


    }
}
