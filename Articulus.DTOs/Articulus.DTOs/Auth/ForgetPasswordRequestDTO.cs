using System.ComponentModel.DataAnnotations;

namespace Articulus.DTOs.Auth
{
    public class ForgetPasswordRequestDTO
    {
        [Required]
        public required string Email { get; set; }
    }
}
