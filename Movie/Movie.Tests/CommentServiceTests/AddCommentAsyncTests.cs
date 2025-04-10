using FluentAssertions;
using Moq;
using Movie.Core.Entities;
using Xunit;

namespace Movie.Tests.CommentServiceTests
{
    public partial class CommentServiceTests
    {
        public class AddCommentAsyncTests : CommentServiceTests
        {
            [Fact]
            public async Task AddCommentAsync_ShouldSetCreatedAtToCurrentTime()
            {
                // Arrange
                var comment = new CommentEntity
                {
                    Id = 0,
                    MovieId = 1,
                    UserId = "user123",
                    Content = "Great movie!"
                };

                var beforeTest = DateTime.UtcNow;
                _commentRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CommentEntity>()))
                    .ReturnsAsync((CommentEntity c) => c);

                // Act
                var result = await _commentService.AddCommentAsync(comment);

                // Assert
                result.CreatedAt.Should().BeOnOrAfter(beforeTest);
                result.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
            }

            [Fact]
            public async Task AddCommentAsync_ShouldCallRepositorySaveChanges()
            {
                // Arrange
                var comment = new CommentEntity
                {
                    Id = 0,
                    MovieId = 1,
                    UserId = "user123",
                    Content = "Great movie!"
                };

                _commentRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CommentEntity>()))
                    .ReturnsAsync((CommentEntity c) => c);

                // Act
                await _commentService.AddCommentAsync(comment);

                // Assert
                _commentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task AddCommentAsync_ShouldReturnAddedComment()
            {
                // Arrange
                var comment = new CommentEntity
                {
                    Id = 0,
                    MovieId = 1,
                    UserId = "user123",
                    Content = "Great movie!"
                };

                var expectedComment = new CommentEntity
                {
                    Id = 1,
                    MovieId = 1,
                    UserId = "user123",
                    Content = "Great movie!"
                };

                _commentRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CommentEntity>()))
                    .ReturnsAsync(expectedComment);

                // Act
                var result = await _commentService.AddCommentAsync(comment);

                // Assert
                result.Should().BeEquivalentTo(expectedComment);
            }
        }
    }
}