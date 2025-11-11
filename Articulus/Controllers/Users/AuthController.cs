using Microsoft.AspNetCore.Mvc;
using Articulus.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Articulus.BLL.Users.Interfaces;
using Articulus.BLL.Exceptions.Auth_Exceptions;
using Articulus.BLL.Exceptions;
using Articulus.BLL.Exceptions.AuthExceptions;

namespace Articulus.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authServices;
        public AuthController(IAuthService authServices)
        {
            _authServices = authServices;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequestDTO userCred)
        {
            try
            {
                var result = await _authServices.RegisterUserAsync(userCred);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserAlreadyExistsException => BadRequest("User with the same email or username already exists."),
                    WeakPasswordException => BadRequest("Password is not strong enough. It should be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character."),
                    OtpLimitExceededException => BadRequest("OTP already sent. Please wait before requesting again."),
                    InvalidCredentialsException => BadRequest("Invalid registration request."),
                    _ => BadRequest("Invalid registration request."),
                };
            }
        }

        //verify email and otp
        [HttpPost("Register/Verify")]
        [Authorize]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyOptRequestDTO verifyReq)
        {
            //get email from token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _authServices.VerifyEmailAsync(email, verifyReq);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    InvalidOtpException => BadRequest("Invalid OTP."),
                    OtpExpiredException => BadRequest("OTP expired. Please request a new one."),
                    UserNotFoundException => BadRequest("Temporary user not found. Please register again."),
                    _ => BadRequest("Invalid verification request."),
                };
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequestDTO loginReq)
        {
            try
            {
                var result = await _authServices.LoginUserAsync(loginReq);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => BadRequest("User not found."),
                    InvalidCredentialsException => BadRequest("Invalid email or password."),
                    _ => BadRequest("Invalid login request."),
                };
            }
        }

        //forgot password
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordRequestDTO forgetPasswordRequest)
        {
            try
            {
                var result = await _authServices.ForgetPasswordAsync(forgetPasswordRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => BadRequest("User not found."),
                    OtpLimitExceededException => BadRequest("OTP already sent. Please wait before requesting again."),
                    InvalidCredentialsException => BadRequest("Invalid request."),
                    _ => BadRequest("Invalid request."),
                };
            }
        }

        //reset password
        [HttpPost("ResetPassword")]
        [Authorize]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO resetPasswordRequest)
        {
            //get email from token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Unauthorized();
            }

            try
            {
                await _authServices.ResetPasswordAsync(email, resetPasswordRequest);
                return Ok("Password reset successful.");
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => BadRequest("User not found."),
                    InvalidCredentialsException => BadRequest("new password is not equal to the confirm password."),
                    WeakPasswordException => BadRequest("Password is not strong enough. It should be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character."),
                    InvalidOtpException => BadRequest("Invalid OTP."),
                    OtpExpiredException => BadRequest("OTP expired. Please request a new one."),
                    _ => BadRequest("Invalid request."),
                };
            }
        }

        //resend otp
        [HttpGet("ResendOtp")]
        [Authorize]
        public async Task<IActionResult> ResendOtp()
        {
            //get email from token
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Unauthorized();
            }

            try
            {
                await _authServices.ResendOtpAsync(email);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => BadRequest("No OTP request found. Please initiate forgot password process."),
                    OtpLimitExceededException => BadRequest("OTP already sent. Please wait before requesting again."),
                    _ => BadRequest("Invalid request."),
                };
            }
            return Ok("OTP resent successfully.");
        }
    }
}
