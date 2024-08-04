using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Exceptions.EmailVerification;
using MovieBookingBackend.Exceptions.User;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Users;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MovieBookingBackend.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/auth")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuthService _userAuthService;
        private readonly IUserService _userService;
        private readonly ILogger<UserAuthController> _logger;
        private readonly IEmailSender _emailSenderService;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IMapper _mapper;

        public UserAuthController(IUserAuthService userAuthService, IUserService userService, ILogger<UserAuthController> logger, IEmailSender emailSenderService, IEmailVerificationService emailVerificationService, IMapper mapper)
        {
            _userAuthService = userAuthService;
            _userService = userService;
            _logger = logger;
            _emailSenderService = emailSenderService;
            _emailVerificationService = emailVerificationService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> RegisterUser(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                UserDTO userDTO = await _userAuthService.Register(userRegisterDTO);
                await _emailVerificationService.CreateEmailVerification(userDTO.Id);
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> RegisterAdmin(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                UserDTO userDTO = await _userAuthService.RegisterAdmin(userRegisterDTO);
                await _emailVerificationService.CreateEmailVerification(userDTO.Id);
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserLoginReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserLoginReturnDTO>> Login(UserLoginDTO userLoginDTO)
        {
            try
            {
                UserLoginReturnDTO loginReturnDTO = await _userAuthService.Login(userLoginDTO);
                Response.Cookies.Append("aceTickets-token", loginReturnDTO.Token, new CookieOptions
                {
                    Secure = true,
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    MaxAge = TimeSpan.FromHours(8),
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });
                return Ok(loginReturnDTO);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        [Authorize]
        [HttpPut("password")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> UpdatePassword(UserLoginDTO userLoginDTO)
        {
            try
            {
                UserDTO userDTO = await _userAuthService.UpdatePassword(userLoginDTO);
                await _emailVerificationService.CreateEmailVerification(userDTO.Id);
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unable to update password {ex}");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        [Authorize]
        [HttpPost("logout")]
        [EnableCors("AllowSpecificOrigin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Logout()
        {
            try
            {
                if (Request.Cookies["aceTickets-token"] != null)
                {
                    Response.Cookies.Delete("aceTickets-token", new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.None,
                        Secure = true,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(-1)
                    });
                }
                return Ok(new { Message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("verify/generateCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateVerificationCode(string userEmail)
        {
            try
            {
                var user = await _userService.GetUserByEmail(userEmail);
                await _emailVerificationService.CreateEmailVerification(user.Id);
                return Ok(new { message = "Verification code generated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("verify/verifyCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyCode(string userEmail, [RegularExpression(@"^\d{6}$")] string verificationCode)
        {
            try
            {
                var user = await _userService.GetUserByEmail(userEmail);
                var isSuccess = await _emailVerificationService.VerifyEmail(user.Id, verificationCode);

                return Ok(new { message = "Verified successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("user")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetUser()
        {
            try
            {
                var result = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine(result);
                var user = await _userService.GetUserById(int.Parse(result));
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(500, ex.Message));
            }
        }
    }
}
