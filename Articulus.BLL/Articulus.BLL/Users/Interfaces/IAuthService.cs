using Articulus.DTOs.Auth;
namespace Articulus.BLL.Users.Interfaces
{
    public interface IAuthService
    {
        public Task<JwtResponseDTO> RegisterUserAsync(RegisterRequestDTO userCred);
        public Task<JwtResponseDTO> VerifyEmailAsync(string email, VerifyOptRequestDTO verifyReq);
        public Task<JwtResponseDTO> LoginUserAsync(LoginRequestDTO loginReq);
        public Task<ForgetPasswordResponseDTO> ForgetPasswordAsync(ForgetPasswordRequestDTO forgetReq);
        public Task ResetPasswordAsync(string email, ResetPasswordRequestDTO resetReq);
        public Task ResendOtpAsync(string email);
    }
}
