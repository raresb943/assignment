using System.ComponentModel.DataAnnotations;

namespace Movie.Core.Models
{
    public class CommentRequest
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}