namespace Articulus.DTOs.Auth
{
    public class ForgetPasswordResponseDTO
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string Message { get; set; }
    }
}
