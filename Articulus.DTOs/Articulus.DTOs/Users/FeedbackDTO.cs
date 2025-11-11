using System.ComponentModel.DataAnnotations;
using Articulus.Data.Models;

namespace Articulus.DTOs.Users
{
    public class FeedbackDTO
    {
        public string Text { get; set; } = null!;
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
    }
}
