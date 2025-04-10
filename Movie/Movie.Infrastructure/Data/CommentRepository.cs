using Microsoft.EntityFrameworkCore;
using Movie.Core.Entities;
using Movie.Core.Interfaces;

namespace Movie.Infrastructure.Data
{
    public class CommentRepository : ICommentRepository
    {
        private readonly MovieDbContext _context;

        public CommentRepository(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<CommentEntity> AddAsync(CommentEntity comment)
        {
            await _context.Comments.AddAsync(comment);
            return comment;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (comment == null)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            return true;
        }

        public async Task<IEnumerable<CommentEntity>> GetByMovieIdAsync(int movieId)
        {
            return await _context.Comments
                .Where(c => c.MovieId == movieId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}