using FluentAssertions;
using Moq;
using Movie.Core.Entities;
using Movie.Core.Interfaces;
using Movie.Infrastructure.Services;
using Xunit;

namespace Movie.Tests.CommentServiceTests
{
    public partial class CommentServiceTests
    {
        protected readonly Mock<ICommentRepository> _commentRepositoryMock;
        protected readonly CommentService _commentService;

        protected CommentServiceTests()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _commentService = new CommentService(_commentRepositoryMock.Object);
        }
    }
}