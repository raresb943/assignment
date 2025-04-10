using Movie.Core.Entities;

namespace Movie.Core.Models
{
    public class SearchResultModel
    {
        public List<MovieEntity> Results { get; set; } = new();
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
    }
}