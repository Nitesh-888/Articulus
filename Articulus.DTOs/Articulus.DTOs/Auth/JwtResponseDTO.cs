using System.Security.Principal;

namespace Articulus.DTOs.Auth
{
    public class JwtResponseDTO
    {
        public required string Token { get; set; }
        public required string Message { get; set; }
    }
}
