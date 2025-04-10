using Microsoft.Extensions.Configuration;
using Movie.Core.Entities;
using Movie.Core.Interfaces;
using Movie.Core.Models;
using System.Text.Json;

namespace Movie.Infrastructure.Services
{
    public class MovieService : IMovieService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.themoviedb.org/3";
        private readonly string _imageBaseUrl = "https://image.tmdb.org/t/p/";
        private readonly ICommentRepository _commentRepository;

        public MovieService(HttpClient httpClient, IConfiguration configuration, ICommentRepository commentRepository)
        {
            _httpClient = httpClient;
            _apiKey = configuration["MovieDbApi:ApiKey"] ?? throw new ArgumentNullException("API Key is missing");
            _commentRepository = commentRepository;
        }

        public async Task<IEnumerable<MovieEntity>> GetLatestMoviesAsync()
        {
            var url = $"{_baseUrl}/movie/now_playing?api_key={_apiKey}&language=en-US&page=1";
            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponseModel>(response, options);

            return result?.Results.Select(MapToMovieEntity) ?? Enumerable.Empty<MovieEntity>();
        }

        public async Task<IEnumerable<MovieEntity>> GetTopRatedMoviesAsync()
        {
            var url = $"{_baseUrl}/movie/top_rated?api_key={_apiKey}&language=en-US&page=1";
            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponseModel>(response, options);

            return result?.Results.Select(MapToMovieEntity) ?? Enumerable.Empty<MovieEntity>();
        }

        public async Task<MovieDetailsModel> GetMovieDetailsAsync(int id)
        {
            var url = $"{_baseUrl}/movie/{id}?api_key={_apiKey}&language=en-US&append_to_response=credits,images";
            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var movieDetails = JsonSerializer.Deserialize<MovieDetailApiResponse>(response, options);

            if (movieDetails == null)
            {
                return null;
            }

            var comments = await _commentRepository.GetByMovieIdAsync(id);

            return new MovieDetailsModel
            {
                Movie = MapToMovieEntity(movieDetails),
                Cast = movieDetails.Credits?.Cast?.Select(c => new CastMemberEntity
                {
                    Id = c.Id,
                    Name = c.Name,
                    Character = c.Character,
                    ProfilePath = !string.IsNullOrEmpty(c.ProfilePath) ? $"{_imageBaseUrl}w185{c.ProfilePath}" : null
                }).Take(10).ToList() ?? new List<CastMemberEntity>(),

                Images = (movieDetails.Images?.Backdrops?.Select(i => new ImageEntity
                {
                    FilePath = $"{_imageBaseUrl}w780{i.FilePath}",
                    Width = i.Width,
                    Height = i.Height,
                    Type = "backdrop"
                }).Take(5).ToList() ?? new List<ImageEntity>())
                .Concat(movieDetails.Images?.Posters?.Select(i => new ImageEntity
                {
                    FilePath = $"{_imageBaseUrl}w342{i.FilePath}",
                    Width = i.Width,
                    Height = i.Height,
                    Type = "poster"
                }).Take(5) ?? Enumerable.Empty<ImageEntity>())
                .ToList(),

                Comments = comments?.ToList() ?? new List<CommentEntity>()
            };
        }

        public async Task<SearchResultModel> SearchMoviesAsync(string query, int? genreId = null, int page = 1)
        {
            string url;

            if (string.IsNullOrWhiteSpace(query) && genreId.HasValue)
            {
                url = $"{_baseUrl}/discover/movie?api_key={_apiKey}&with_genres={genreId}&page={page}";
            }
            else if (!string.IsNullOrWhiteSpace(query) && genreId.HasValue)
            {
                url = $"{_baseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&with_genres={genreId}&page={page}";
            }
            else
            {
                url = $"{_baseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}";
            }

            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponseModel>(response, options);

            if (result == null)
            {
                return new SearchResultModel();
            }

            return new SearchResultModel
            {
                Results = result.Results.Select(MapToMovieEntity).ToList(),
                Page = result.Page,
                TotalPages = result.TotalPages,
                TotalResults = result.TotalResults
            };
        }

        public async Task<IEnumerable<GenreEntity>> GetGenresAsync()
        {
            var url = $"{_baseUrl}/genre/movie/list?api_key={_apiKey}&language=en-US";
            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<GenreResponseModel>(response, options);

            return result?.Genres?.Select(g => new GenreEntity
            {
                Id = g.Id,
                Name = g.Name
            }) ?? Enumerable.Empty<GenreEntity>();
        }

        private MovieEntity MapToMovieEntity(MovieApiModel movie)
        {
            return new MovieEntity
            {
                Id = movie.Id,
                Title = movie.Title,
                Overview = movie.Overview,
                PosterPath = !string.IsNullOrEmpty(movie.PosterPath) ? $"{_imageBaseUrl}w342{movie.PosterPath}" : null,
                BackdropPath = !string.IsNullOrEmpty(movie.BackdropPath) ? $"{_imageBaseUrl}w780{movie.BackdropPath}" : null,
                VoteAverage = movie.VoteAverage,
                VoteCount = movie.VoteCount,
                ReleaseDate = DateTime.TryParse(movie.ReleaseDate, out var releaseDate) ? releaseDate : DateTime.MinValue,
                Genres = movie.Genres?.Select(g => new GenreEntity
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToList()
            };
        }

        private class ApiResponseModel
        {
            public List<MovieApiModel> Results { get; set; } = new();
            public int Page { get; set; }
            public int TotalPages { get; set; }
            public int TotalResults { get; set; }
        }

        private class MovieApiModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Overview { get; set; }
            public string PosterPath { get; set; }
            public string BackdropPath { get; set; }
            public double VoteAverage { get; set; }
            public int VoteCount { get; set; }
            public string ReleaseDate { get; set; }
            public List<GenreApiModel> Genres { get; set; }
        }

        private class GenreApiModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class GenreResponseModel
        {
            public List<GenreApiModel> Genres { get; set; }
        }

        private class MovieDetailApiResponse : MovieApiModel
        {
            public CreditsApiModel Credits { get; set; }
            public ImagesApiModel Images { get; set; }
        }

        private class CreditsApiModel
        {
            public List<CastApiModel> Cast { get; set; }
        }

        private class CastApiModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Character { get; set; }
            public string ProfilePath { get; set; }
        }

        private class ImagesApiModel
        {
            public List<ImageApiModel> Backdrops { get; set; }
            public List<ImageApiModel> Posters { get; set; }
        }

        private class ImageApiModel
        {
            public string FilePath { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}