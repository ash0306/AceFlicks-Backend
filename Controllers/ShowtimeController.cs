using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Exceptions.Showtime;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Showtimes;

namespace MovieBookingBackend.Controllers
{
    [Route("api/showtime")]
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

        [HttpPost("addShowtime")]
        [ProducesResponseType(typeof(ShowtimeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShowtimeDTO>> AddShowtime(AddShowtimeDTO addShowtimeDTO)
        {
            try
            {
                var result = await _showtimeService.AddShowtime(addShowtimeDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        [HttpGet("getAllShowtimes")]
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

        [HttpPut("updateShowtime")]
        [ProducesResponseType(typeof(ShowtimeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShowtimeDTO>> UpdateShowtime(UpdateShowtimeDTO updateShowtimeDTO)
        {
            try
            {
                var result = await _showtimeService.UpdateShowtime(updateShowtimeDTO);
                return Ok(result);
            }
            catch( Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
    }
}
