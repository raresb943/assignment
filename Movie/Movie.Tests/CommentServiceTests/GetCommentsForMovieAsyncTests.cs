using FluentAssertions;
using Moq;
using Movie.Core.Entities;
using Xunit;

namespace Movie.Tests.CommentServiceTests
{
    public partial class CommentServiceTests
    {
        public class GetCommentsForMovieAsyncTests : CommentServiceTests
        {
            [Fact]
            public async Task GetCommentsForMovieAsync_ShouldReturnCommentsFromRepository()
            {
                // Arrange
                int movieId = 1;
                var expectedComments = new List<CommentEntity>
                {
                    new CommentEntity
                    {
                        Id = 1,
                        MovieId = movieId,
                        UserId = "user1",
                        Content = "Great movie!",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new CommentEntity
                    {
                        Id = 2,
                        MovieId = movieId,
                        UserId = "user2",
                        Content = "Loved it!",
                        CreatedAt = DateTime.UtcNow.AddHours(-5)
                    }
                };

                _commentRepositoryMock.Setup(x => x.GetByMovieIdAsync(movieId))
                    .ReturnsAsync(expectedComments);

                // Act
                var result = await _commentService.GetCommentsForMovieAsync(movieId);

                // Assert
                result.Should().BeEquivalentTo(expectedComments);
            }

            [Fact]
            public async Task GetCommentsForMovieAsync_WhenNoComments_ShouldReturnEmptyList()
            {
                // Arrange
                int movieId = 999;
                var emptyList = new List<CommentEntity>();

                _commentRepositoryMock.Setup(x => x.GetByMovieIdAsync(movieId))
                    .ReturnsAsync(emptyList);

                // Act
                var result = await _commentService.GetCommentsForMovieAsync(movieId);

                // Assert
                result.Should().BeEmpty();
            }

            [Fact]
            public async Task GetCommentsForMovieAsync_ShouldCallRepositoryWithCorrectMovieId()
            {
                // Arrange
                int movieId = 123;

                _commentRepositoryMock.Setup(x => x.GetByMovieIdAsync(movieId))
                    .ReturnsAsync(new List<CommentEntity>());

                // Act
                await _commentService.GetCommentsForMovieAsync(movieId);

                // Assert
                _commentRepositoryMock.Verify(x => x.GetByMovieIdAsync(movieId), Times.Once);
            }
        }
    }
}