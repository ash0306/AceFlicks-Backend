using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Exceptions.User;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Users;

namespace MovieBookingBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuthService _userAuthService;
        private readonly ILogger<UserAuthController> _logger;

        public UserAuthController(IUserAuthService userAuthService, ILogger<UserAuthController> logger)
        {
            _userAuthService = userAuthService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> RegisterUser(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                UserDTO userDTO = await _userAuthService.Register(userRegisterDTO);
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
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unable to update password {ex}");
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }
    }
}
