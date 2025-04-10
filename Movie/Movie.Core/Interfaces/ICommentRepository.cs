using Movie.Core.Entities;

namespace Movie.Core.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<CommentEntity>> GetByMovieIdAsync(int movieId);
        Task<CommentEntity> AddAsync(CommentEntity comment);
        Task<bool> DeleteAsync(int id, string userId);
        Task<bool> SaveChangesAsync();
    }
}