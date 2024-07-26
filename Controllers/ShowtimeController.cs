using AutoMapper;
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
    public class ShowtimeController : ControllerBase
    {
        private readonly IShowtimeService _showtimeService;
        private readonly ILogger<ShowtimeController> _logger;

        public ShowtimeController(IShowtimeService showtimeService, ILogger<ShowtimeController> logger)
        {
            _showtimeService = showtimeService;
            _logger = logger;
        }

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
        [ProducesResponseType(typeof(IEnumerable<ShowtimeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ShowtimeDTO>>> GetMovieShowtimes(string movieName)
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
        public async Task<ActionResult<IEnumerable<IGrouping<int, ShowtimeDTO>>>> GetShowtimesByTheatre(string theatreName)
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
    }
}
