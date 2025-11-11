using Articulus.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Articulus.DTOs.Articles
{
    public class GetAllArticlesResponseDTO
    {
        public required Guid ArticleId { get; set; }
        public required string Title { get; set; }
        public required string? CoverImageUrl { get; set; }
        public required int ViewsCount { get; set; }
        public int ReactionsCount { get; set; }
        public int CommentsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
