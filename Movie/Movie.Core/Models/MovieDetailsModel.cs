using Movie.Core.Entities;

namespace Movie.Core.Models
{
    public class MovieDetailsModel
    {
        public MovieEntity Movie { get; set; }
        public List<CastMemberEntity> Cast { get; set; } = new();
        public List<ImageEntity> Images { get; set; } = new();
        public List<CommentEntity> Comments { get; set; } = new();
    }
}