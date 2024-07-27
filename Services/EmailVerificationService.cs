using AutoMapper;
using MovieBookingBackend.Exceptions.EmailVerification;
using MovieBookingBackend.Exceptions.User;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.Enums;

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

        public EmailVerificationService(IEmailVerificationRepository emailVerificationRepository, IMapper mapper, IConfiguration configuration, IEmailSender emailService, IRepository<int, User> userRepository, ILogger<EmailVerificationService> logger)
        {
            _emailVerificationRepository = emailVerificationRepository;
            _mapper = mapper;
            _configuration = configuration;
            _emailService = emailService;
            _userRepository = userRepository;
            _logger = logger;
        }

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

        private string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
