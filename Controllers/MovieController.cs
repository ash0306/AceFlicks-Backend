using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingBackend.Interfaces;
using MovieBookingBackend.Models;
using MovieBookingBackend.Models.DTOs.Movie;

namespace MovieBookingBackend.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieServices _movieServices;
        private readonly ILogger<MovieController> _logger;

        public MovieController(IMovieServices movieServices, ILogger<MovieController> logger)
        {
            _movieServices = movieServices;
            _logger = logger;
        }

        [HttpPost("addMovie")]
        [ProducesResponseType(typeof(MovieDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MovieDTO>> AddMovie(MovieDTO movieDTO)
        {
            try
            {
                MovieDTO result = await _movieServices.AddMovie(movieDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        [HttpGet("getAllMovies")]
        [ProducesResponseType(typeof(IEnumerable<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetAllMovies()
        {
            try
            {
                var result = await _movieServices.GetAllMovies();
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        [HttpGet("getRunningMovies")]
        [ProducesResponseType(typeof(IEnumerable<MovieDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetAllRunningMovies()
        {
            try
            {
                var result = await _movieServices.GetAllRunningMovies();
                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }

        [HttpPut("updateMovie")]
        [ProducesResponseType(typeof(MovieDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MovieDTO>> updateMovie(UpdateMovieDTO movieDTO)
        {
            try
            {
                var result = await _movieServices.UpdateMovie(movieDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message, ex);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }
    }
}
