using Movie.Core.Entities;

namespace Movie.Core.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentEntity>> GetCommentsForMovieAsync(int movieId);
        Task<CommentEntity> AddCommentAsync(CommentEntity comment);
        Task<bool> DeleteCommentAsync(int id, string userId);
    }
}