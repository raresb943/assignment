using Microsoft.AspNetCore.Mvc;
using Movie.Core.Interfaces;
using Movie.Core.Models;

namespace Movie.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestMovies()
        {
            var movies = await _movieService.GetLatestMoviesAsync();
            return Ok(movies);
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedMovies()
        {
            var movies = await _movieService.GetTopRatedMoviesAsync();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieDetails(int id)
        {
            var movieDetails = await _movieService.GetMovieDetailsAsync(id);

            if (movieDetails == null)
            {
                return NotFound();
            }

            return Ok(movieDetails);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMovies(
            [FromQuery] string? query,
            [FromQuery] int? genreId,
            [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query) && !genreId.HasValue)
            {
                return BadRequest("Either query or genreId must be provided");
            }

            var searchResults = await _movieService.SearchMoviesAsync(query ?? string.Empty, genreId, page);
            return Ok(searchResults);
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await _movieService.GetGenresAsync();
            return Ok(genres);
        }
    }
}