namespace Movie.Core.Entities
{
    public class MovieEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public string PosterPath { get; set; }
        public string BackdropPath { get; set; }
        public double VoteAverage { get; set; }
        public int VoteCount { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<GenreEntity>? Genres { get; set; }
        public List<CastMemberEntity>? Cast { get; set; }
        public List<ImageEntity>? Images { get; set; }
        public List<CommentEntity>? Comments { get; set; }
    }
}