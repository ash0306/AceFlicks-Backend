using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Showtimes;
using MovieBookingBackend.Models.DTOs.Theatres;

namespace MovieBookingBackend.Controllers
{
    [Route("api/theatre")]
    [ApiController]
    public class TheatreController : ControllerBase
    {
        private readonly ITheatreService _theatreService;
        private readonly ILogger<TheatreController> _logger;

        public TheatreController(ITheatreService theatreService, ILogger<TheatreController> logger)
        {
            _logger = logger;
            _theatreService = theatreService;
        }

        [HttpPost("addTheatre")]
        [ProducesResponseType(typeof(TheatreDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TheatreDTO>> AddTheatre(TheatreDTO theatreDTO)
        {
            try
            {
                var result = await _theatreService.AddTheatre(theatreDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        [HttpGet("getAllTheatres")]
        [ProducesResponseType(typeof(IEnumerable<TheatreDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TheatreDTO>>> GetAllTheatres()
        {
            try
            {
                var result = await _theatreService.GetAllTheatres();
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        [HttpGet("getTheatreById")]
        [ProducesResponseType(typeof(TheatreDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TheatreDTO>> GetTheatreById(int id)
        {
            try
            {
                var result = await _theatreService.GetTheatreById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        [HttpGet("getShowtimeByTheatre")]
        [ProducesResponseType(typeof(IEnumerable<IGrouping<int, ShowtimeDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<IGrouping<int, ShowtimeDTO>>>> GetShowtimesByTheatre(string theatreName)
        {
            try
            {
                var result = await _theatreService.GetShowtimesForATheatre(theatreName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        [HttpGet("getTheatreLocations")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<string>>> GetTheatreLocations(string theatreName)
        {
            try
            {
                var result = await _theatreService.GetTheatreLocationsByName(theatreName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        [HttpPut("updateTheatre")]
        [ProducesResponseType(typeof(TheatreDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TheatreDTO>> updateTheatre(UpdateTheatreDTO updateTheatreDTO)
        {
            try
            {
                var result = await _theatreService.UpdateTheatre(updateTheatreDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }
    }
}
