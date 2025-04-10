using FluentAssertions;
using Moq;
using Movie.Core.Entities;
using Movie.Core.Models;
using Xunit;

namespace Movie.Tests.MovieServiceTests
{
    public partial class MovieServiceTests
    {
        public class GetMovieDetailsAsyncTests : MovieServiceTests
        {
            [Fact]
            public async Task GetMovieDetailsAsync_ShouldReturnMovieDetailsModel()
            {
                // Arrange
                int movieId = 123;
                var jsonResponse = @"
                {
                    ""id"": 123,
                    ""title"": ""Test Movie"",
                    ""overview"": ""Test overview"",
                    ""poster_path"": ""/poster.jpg"",
                    ""backdrop_path"": ""/backdrop.jpg"",
                    ""vote_average"": 8.5,
                    ""vote_count"": 3000,
                    ""release_date"": ""2023-01-01"",
                    ""genres"": [
                        { ""id"": 1, ""name"": ""Action"" },
                        { ""id"": 2, ""name"": ""Sci-Fi"" }
                    ],
                    ""credits"": {
                        ""cast"": [
                            {
                                ""id"": 101,
                                ""name"": ""Actor One"",
                                ""character"": ""Character One"",
                                ""profile_path"": ""/profile1.jpg""
                            },
                            {
                                ""id"": 102,
                                ""name"": ""Actor Two"",
                                ""character"": ""Character Two"",
                                ""profile_path"": ""/profile2.jpg""
                            }
                        ]
                    },
                    ""images"": {
                        ""backdrops"": [
                            {
                                ""file_path"": ""/backdrop1.jpg"",
                                ""width"": 1920,
                                ""height"": 1080
                            }
                        ],
                        ""posters"": [
                            {
                                ""file_path"": ""/poster1.jpg"",
                                ""width"": 500,
                                ""height"": 750
                            }
                        ]
                    }
                }";

                var comments = new List<CommentEntity>
                {
                    new CommentEntity
                    {
                        Id = 1,
                        MovieId = movieId,
                        UserId = "user1",
                        Content = "Great movie!",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                };

                SetupHttpMessageHandlerMock($"movie/{movieId}", jsonResponse);

                _commentRepositoryMock.Setup(x => x.GetByMovieIdAsync(movieId))
                    .ReturnsAsync(comments);

                // Act
                var result = await _movieService.GetMovieDetailsAsync(movieId);

                // Assert
                result.Should().NotBeNull();
                result.Movie.Id.Should().Be(movieId);
                result.Movie.Title.Should().Be("Test Movie");
                result.Cast.Should().HaveCount(2);
                result.Images.Should().HaveCount(2);
                result.Comments.Should().HaveCount(1);
            }

            [Fact]
            public async Task GetMovieDetailsAsync_ShouldCallCommentRepository()
            {
                // Arrange
                int movieId = 123;
                var jsonResponse = @"
                {
                    ""id"": 123,
                    ""title"": ""Test Movie"",
                    ""overview"": """",
                    ""poster_path"": null,
                    ""backdrop_path"": null,
                    ""vote_average"": 0,
                    ""vote_count"": 0,
                    ""release_date"": """",
                    ""credits"": { ""cast"": [] },
                    ""images"": { ""backdrops"": [], ""posters"": [] }
                }";

                SetupHttpMessageHandlerMock($"movie/{movieId}", jsonResponse);
                _commentRepositoryMock.Setup(x => x.GetByMovieIdAsync(movieId))
                    .ReturnsAsync(new List<CommentEntity>());

                // Act
                await _movieService.GetMovieDetailsAsync(movieId);

                // Assert
                _commentRepositoryMock.Verify(x => x.GetByMovieIdAsync(movieId), Times.Once);
            }

            [Fact]
            public async Task GetMovieDetailsAsync_WhenMovieHasNullImagePaths_ShouldHandleGracefully()
            {
                // Arrange
                int movieId = 123;
                var jsonResponse = @"
                {
                    ""id"": 123,
                    ""title"": ""Test Movie"",
                    ""overview"": ""Test overview"",
                    ""poster_path"": null,
                    ""backdrop_path"": null,
                    ""vote_average"": 8.5,
                    ""vote_count"": 3000,
                    ""release_date"": ""2023-01-01"",
                    ""credits"": { ""cast"": [] },
                    ""images"": { ""backdrops"": [], ""posters"": [] }
                }";

                SetupHttpMessageHandlerMock($"movie/{movieId}", jsonResponse);
                _commentRepositoryMock.Setup(x => x.GetByMovieIdAsync(movieId))
                    .ReturnsAsync(new List<CommentEntity>());

                // Act
                var result = await _movieService.GetMovieDetailsAsync(movieId);

                // Assert
                result.Should().NotBeNull();
                result.Movie.PosterPath.Should().BeNull();
                result.Movie.BackdropPath.Should().BeNull();
            }
        }
    }
}