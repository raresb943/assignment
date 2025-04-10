using Movie.Core.Entities;
using Movie.Core.Interfaces;

namespace Movie.Infrastructure.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<CommentEntity> AddCommentAsync(CommentEntity comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            var result = await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> DeleteCommentAsync(int id, string userId)
        {
            var deleted = await _commentRepository.DeleteAsync(id, userId);
            if (deleted)
            {
                await _commentRepository.SaveChangesAsync();
            }
            return deleted;
        }

        public async Task<IEnumerable<CommentEntity>> GetCommentsForMovieAsync(int movieId)
        {
            return await _commentRepository.GetByMovieIdAsync(movieId);
        }
    }
}