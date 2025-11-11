using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using Articulus.Models;

namespace Articulus.Data.Models
{
    public class Article
    {
        [Key, Required]
        public Guid ArticleId { get; set; }

        [Required]
        public  required string Title { get; set; }

        public string? CoverImageUrl { get; set; }

        [Required]
        public required string Content { get; set; }

        //Navigatioin property
        [Required]
        public required Guid UserId { get; set; }
        public required User User { get; set; }


        public ICollection<Comment> Comments { get; set; } = [];
        public ICollection<Report> Reports { get; set; } = [];

        public ICollection<Reaction> Reactions { get; set; } = [];

        public ICollection<Bookmark> Bookmarks { get; set; } = [];
        public ICollection<View> Views { get; set; } = [];


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set;} = DateTime.Now;

    }
}
