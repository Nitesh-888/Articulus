namespace Articulus.DTOs.Auth
{
    public class VerifyOptRequestDTO
    {
        public required int Otp { get; set; }
        public required string TimeZone { get; set; }
    }
}
