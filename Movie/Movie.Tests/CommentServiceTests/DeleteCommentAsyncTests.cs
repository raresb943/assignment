using FluentAssertions;
using Moq;
using Xunit;

namespace Movie.Tests.CommentServiceTests
{
    public partial class CommentServiceTests
    {
        public class DeleteCommentAsyncTests : CommentServiceTests
        {
            [Fact]
            public async Task DeleteCommentAsync_WhenSuccessful_ShouldReturnTrue()
            {
                // Arrange
                int commentId = 1;
                string userId = "user123";

                _commentRepositoryMock.Setup(x => x.DeleteAsync(commentId, userId))
                    .ReturnsAsync(true);

                // Act
                var result = await _commentService.DeleteCommentAsync(commentId, userId);

                // Assert
                result.Should().BeTrue();
            }

            [Fact]
            public async Task DeleteCommentAsync_WhenSuccessful_ShouldCallSaveChanges()
            {
                // Arrange
                int commentId = 1;
                string userId = "user123";

                _commentRepositoryMock.Setup(x => x.DeleteAsync(commentId, userId))
                    .ReturnsAsync(true);

                // Act
                await _commentService.DeleteCommentAsync(commentId, userId);

                // Assert
                _commentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            }

            [Fact]
            public async Task DeleteCommentAsync_WhenNotFound_ShouldReturnFalse()
            {
                // Arrange
                int commentId = 999;
                string userId = "user123";

                _commentRepositoryMock.Setup(x => x.DeleteAsync(commentId, userId))
                    .ReturnsAsync(false);

                // Act
                var result = await _commentService.DeleteCommentAsync(commentId, userId);

                // Assert
                result.Should().BeFalse();
            }

            [Fact]
            public async Task DeleteCommentAsync_WhenNotFound_ShouldNotCallSaveChanges()
            {
                // Arrange
                int commentId = 999;
                string userId = "user123";

                _commentRepositoryMock.Setup(x => x.DeleteAsync(commentId, userId))
                    .ReturnsAsync(false);

                // Act
                await _commentService.DeleteCommentAsync(commentId, userId);

                // Assert
                _commentRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Never);
            }
        }
    }
}