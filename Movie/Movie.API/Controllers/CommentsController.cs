using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie.Core.Entities;
using Movie.Core.Interfaces;
using Movie.Core.Models;
using System.Security.Claims;

namespace Movie.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetCommentsByMovie(int movieId)
        {
            var comments = await _commentService.GetCommentsForMovieAsync(movieId);
            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(CommentRequest request)
        {
            var comment = new CommentEntity
            {
                MovieId = request.MovieId,
                Content = request.Content,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                UserName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown User"
            };

            if (string.IsNullOrEmpty(comment.UserId))
            {
                return BadRequest(new { message = "User identity missing from token" });
            }

            var result = await _commentService.AddCommentAsync(comment);
            return CreatedAtAction(nameof(GetCommentsByMovie), new { movieId = comment.MovieId }, result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User identity missing from token" });
            }

            var result = await _commentService.DeleteCommentAsync(id, userId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}