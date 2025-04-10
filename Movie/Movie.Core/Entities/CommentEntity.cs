namespace Movie.Core.Entities
{
    public class CommentEntity
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}