using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Exceptions.Showtime;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Seats;
using MovieBookingBackend.Models.DTOs.Showtimes;
using MovieBookingBackend.Services;

namespace MovieBookingBackend.Controllers
{
    [Route("api/showtimes")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;
        private readonly ILogger<ShowtimeController> _logger;

        public ShowtimeController(IShowtimeService showtimeService, ILogger<ShowtimeController> logger)
        {
            _showtimeService = showtimeService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(ShowtimeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShowtimeDTO>> AddShowtime(AddShowtimeDTO addShowtimeDTO)
        {
            try
            {
                var result = await _showtimeService.AddShowtime(addShowtimeDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ShowtimeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ShowtimeDTO>>> GetAllShowtimes()
        {
            try
            {
                var result = await _showtimeService.GetAllShowtime();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(typeof(ShowtimeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShowtimeDTO>> UpdateShowtime(UpdateShowtimeDTO updateShowtimeDTO)
        {
            try
            {
                var result = await _showtimeService.UpdateShowtime(updateShowtimeDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }


        [HttpGet("movie/{movieName}")]
        [ProducesResponseType(typeof(IEnumerable<ShowtimeGroupDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ShowtimeGroupDTO>>> GetMovieShowtimes(string movieName)
        {
            try
            {
                var result = await _showtimeService.GetShowtimesForAMovie(movieName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }


        [HttpGet("theatre/{theatreName}")]
        [ProducesResponseType(typeof(IEnumerable<IGrouping<int, ShowtimeDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ShowtimeGroupDTO>>> GetShowtimesByTheatre(string theatreName)
        {
            try
            {
                var result = await _showtimeService.GetShowtimesForATheatre(theatreName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet("seats/{showtimeId}")]
        [ProducesResponseType(typeof(IEnumerable<SeatDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SeatDTO>>> GetSeatsByShowtimeId(int showtimeId)
        {
            try
            {
                var result = await _showtimeService.GetSeatsByShowtime(showtimeId);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet("{showtimeId}")]
        [ProducesResponseType(typeof(ShowtimeDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShowtimeDetailsDTO>> GetShowtimeDetailsById(int showtimeId)
        {
            try
            {
                var result = await _showtimeService.GetShowtimeDetailsById(showtimeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> DeleteShowtime(int showtimeId)
        {
            try
            {
                var result = await _showtimeService.DeleteShowtime(showtimeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }


    }
}
