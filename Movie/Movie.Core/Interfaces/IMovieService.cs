using Movie.Core.Entities;
using Movie.Core.Models;

namespace Movie.Core.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieEntity>> GetLatestMoviesAsync();
        Task<IEnumerable<MovieEntity>> GetTopRatedMoviesAsync();
        Task<MovieDetailsModel> GetMovieDetailsAsync(int id);
        Task<SearchResultModel> SearchMoviesAsync(string query, int? genreId = null, int page = 1);
        Task<IEnumerable<GenreEntity>> GetGenresAsync();
    }
}