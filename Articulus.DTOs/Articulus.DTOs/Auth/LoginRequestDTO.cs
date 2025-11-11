using System.ComponentModel.DataAnnotations;

namespace Articulus.DTOs.Auth
{
    public class LoginRequestDTO
    {
        public required string UsernameOrEmail { get; set; }

        public required string Password { get; set; }
    }
}
