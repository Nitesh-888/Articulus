using System.ComponentModel.DataAnnotations;

namespace Articulus.DTOs.Comments
{
    public class CreateCommentRequestDTO
    {
        [Required]
        public required String Text { get; set; }
    }
}
