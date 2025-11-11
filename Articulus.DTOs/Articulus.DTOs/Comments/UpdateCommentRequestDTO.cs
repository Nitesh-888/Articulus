using System.ComponentModel.DataAnnotations;

namespace Articulus.DTOs.Comments
{
    public class UpdateCommentRequestDTO
    {
        [Required]
        public required String Text { get; set; }
    }
}
