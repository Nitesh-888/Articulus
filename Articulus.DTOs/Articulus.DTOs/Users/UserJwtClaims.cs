namespace Articulus.DTOs.Users
{
    public class UserJwtClaims
    {
        //Dto for user jwt claims
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public required string TimeZone { get; set; }
    }
}
