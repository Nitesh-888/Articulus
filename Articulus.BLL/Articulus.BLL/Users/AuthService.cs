using Articulus.DTOs.Auth;
using Articulus.BLL.Users.Interfaces;
using Articulus.Data;
using Microsoft.EntityFrameworkCore;
using Articulus.Services;
using Microsoft.AspNetCore.Identity;
using Articulus.Data.Models;
using Articulus.BLL.Exceptions.Auth_Exceptions;
using Articulus.BLL.Exceptions;
using Articulus.BLL.Exceptions.AuthExceptions;

namespace Articulus.BLL.Users
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly JwtService _jwtService;
        private readonly MailService _mailServices;
        private readonly PasswordHasher<TempUser> _tempUserPasswordHasher = new();
        private readonly PasswordHasher<UserCred> _userPasswordHasher = new();
        public AuthService(AppDbContext dbContext, JwtService jwtService, MailService mailService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
            _mailServices = mailService;
        }

        public async Task<JwtResponseDTO> RegisterUserAsync(RegisterRequestDTO userCred)
        {
            if (userCred == null || userCred.Email == null || userCred.Username == null || userCred.Password == null)
            {
                throw new InvalidCredentialsException();
            }
            //unique username
            var userName = await _dbContext.UserCreds.SingleOrDefaultAsync(u => u.Username == userCred.Username);
            if (userName != null)
            {
                throw new UserAlreadyExistsException(userCred.Username);
            }

            //unique email
            var email = await _dbContext.UserCreds.SingleOrDefaultAsync(u => u.Email == userCred.Email);
            if (email != null)
            {
                throw new UserAlreadyExistsException(userCred.Email);
            }

            //check password strength
            if (!_jwtService.IsPasswordStrong(userCred.Password))
            {
                // return BadRequest("Password is not strong enough. It should be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
                throw new WeakPasswordException();
            }

            //check if temp user exists with same email or username, if yes delete
            var tempUserExists = await _dbContext.TempUsers.FindAsync(userCred.Email);
            if (tempUserExists != null)
            {
                //check otp resend time 1 minute
                if ((DateTime.UtcNow - tempUserExists.OtpGeneratedAt).TotalMinutes < 1)
                {
                    // return BadRequest("OTP already sent. Please wait before requesting again.");
                    throw new OtpLimitExceededException();
                }
                _dbContext.TempUsers.Remove(tempUserExists);
                await _dbContext.SaveChangesAsync();
            }
            var tempUsernameExists = await _dbContext.TempUsers.SingleOrDefaultAsync(u => u.Username == userCred.Username);
            if (tempUsernameExists != null)
            {
                _dbContext.TempUsers.Remove(tempUsernameExists);
                await _dbContext.SaveChangesAsync();
            }


            //generate otp
            var otp = _jwtService.random.Next(100000, 1000000);

            //send otp email
            await _mailServices.SendOtpEmail(userCred.Email, userCred.Username, otp.ToString());

            //store temp user
            var tempUser = new TempUser()
            {
                Email = userCred.Email,
                Username = userCred.Username,
                Otp = otp,
                OtpGeneratedAt = DateTime.UtcNow
            };
            tempUser.Password = _tempUserPasswordHasher.HashPassword(tempUser, userCred.Password);

            await _dbContext.TempUsers.AddAsync(tempUser);
            await _dbContext.SaveChangesAsync();

            //jwt for otp verification
            var jwtToken = _jwtService.GenerateJwtForEmailVerification(userCred.Email, 10);

            var response = new JwtResponseDTO()
            {
                Token = jwtToken,
                Message = "OTP sent to email. Please verify to complete registration."
            };
            return response;
        }

        public async Task<JwtResponseDTO> VerifyEmailAsync(string email, VerifyOptRequestDTO verifyReq)
        {
            var tempUser = _dbContext.TempUsers.Find(email);
            if (tempUser == null)
            {
                throw new UserNotFoundException(email);
            }
            //check otp expiry 10 minutes
            if ((DateTime.UtcNow - tempUser.OtpGeneratedAt).TotalMinutes > 10)
            {
                _dbContext.TempUsers.Remove(tempUser);
                await _dbContext.SaveChangesAsync();
                throw new OtpExpiredException();
            }

            //check otp
            if (tempUser.Otp != verifyReq.Otp)
            {
                throw new InvalidOtpException();
            }

            //create user
            var newUser = new UserCred()
            {
                Email = tempUser.Email,
                Username = tempUser.Username,
                Password = tempUser.Password,
                User = new User()
                {
                    TimeZone = verifyReq.TimeZone
                }
            };
            //remove temp user
            _dbContext.TempUsers.Remove(tempUser);

            await _dbContext.UserCreds.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            //jwt token
            var jwtToken = _jwtService.GenerateJwtToken(newUser.UserCredId.ToString());

            //response 
            var response = new JwtResponseDTO()
            {
                Token = jwtToken,
                Message = "User registered successfully"
            };
            return response;
        }

        //login user
        public async Task<JwtResponseDTO> LoginUserAsync(LoginRequestDTO loginReq)
        {
            if (loginReq == null || loginReq.Password == null || loginReq.UsernameOrEmail == null)
            {
                throw new InvalidCredentialsException();
            }
            var userCred = await _dbContext.UserCreds.SingleOrDefaultAsync(u => u.Username == loginReq.UsernameOrEmail || u.Email == loginReq.UsernameOrEmail);
            if (userCred == null)
            {
                throw new InvalidCredentialsException();
            }

            var result = _userPasswordHasher.VerifyHashedPassword(userCred, userCred.Password, loginReq.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new InvalidCredentialsException();
            }
            var jwtToken = _jwtService.GenerateJwtToken(userCred.UserCredId.ToString());
            var response = new JwtResponseDTO()
            {
                Token = jwtToken,
                Message = "User logged in successfully"
            };
            return response;
        }

        //forgot password
        public async Task<ForgetPasswordResponseDTO> ForgetPasswordAsync(ForgetPasswordRequestDTO forgetReq)
        {
            if (forgetReq == null || string.IsNullOrEmpty(forgetReq.Email))
            {
                throw new InvalidCredentialsException();
            }

            var user = await _dbContext.UserCreds.SingleOrDefaultAsync(u => u.Email == forgetReq.Email);
            if (user == null)
            {
                throw new UserNotFoundException(forgetReq.Email);
            }
            //check otp resend time 1 minute
            if (user.OtpGeneratedAt != null && (DateTime.UtcNow - user.OtpGeneratedAt.Value).TotalMinutes < 1)
            {
                throw new OtpLimitExceededException();
            }

            // Generate OTP and send email
            var otp = _jwtService.random.Next(100000, 1000000);
            await _mailServices.SendOtpEmail(user.Email, user.Username, otp.ToString());

            // Save OTP to database
            user.Otp = otp;
            user.OtpGeneratedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            // Generate JWT token
            var jwtToken = _jwtService.GenerateJwtForEmailVerification(user.Email, 10);
            var response = new ForgetPasswordResponseDTO()
            {
                Email = user.Email,
                Token = jwtToken,
                Message = "OTP sent to email. Please verify to reset your password."
            };
            return response;
        }

        //reset password
        public async Task ResetPasswordAsync(string email, ResetPasswordRequestDTO resetReq)
        {
            var user = await _dbContext.UserCreds.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new UserNotFoundException(email);
            }

            //check otp expiry 10 minutes
            if (user.Otp == null || user.OtpGeneratedAt == null || (DateTime.UtcNow - user.OtpGeneratedAt.Value).TotalMinutes > 10)
            {
                user.Otp = null;
                user.OtpGeneratedAt = null;
                await _dbContext.SaveChangesAsync();
                throw new OtpExpiredException();
            }

            //check new password and confirm password match
            if (resetReq.NewPassword != resetReq.ConfirmPassword)
            {
                throw new InvalidCredentialsException();
            }

            //check password strength
            if (!_jwtService.IsPasswordStrong(resetReq.NewPassword))
            {
                throw new WeakPasswordException();
            }
            
            //check email is correct
            if (email != resetReq.Email)
            {
                throw new InvalidCredentialsException();
            }

            //check otp
            if (user.Otp != resetReq.Otp)
            {
                throw new InvalidOtpException();
            }

            user.Password = _userPasswordHasher.HashPassword(user, resetReq.NewPassword);
            user.Otp = null;
            user.OtpGeneratedAt = null;
            await _dbContext.SaveChangesAsync();
        }

        //resend otp
        public async Task ResendOtpAsync(string email)
        {
            //Resend otp for forgot password 
            var user = await _dbContext.UserCreds.SingleOrDefaultAsync(u => u.Email == email);
            if (user != null && user.OtpGeneratedAt != null)
            {
                if ((DateTime.UtcNow - user.OtpGeneratedAt.Value).TotalMinutes < 1)
                {
                    throw new OtpLimitExceededException();
                }
                //generate new otp
                var otpForgotPass = _jwtService.random.Next(100000, 1000000);
                //send otp email
                await _mailServices.SendOtpEmail(user.Email, user.Username, otpForgotPass.ToString());
                //update otp and otp generated time
                user.Otp = otpForgotPass;
                user.OtpGeneratedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                return;
            }
            else if (user != null && user.OtpGeneratedAt == null)
            {
                throw new UserNotFoundException(email);
            }

            //Resend otp for registration
            var tempUser = await _dbContext.TempUsers.SingleOrDefaultAsync(u => u.Email == email);
            if (tempUser == null)
            {
                throw new UserNotFoundException(email);
            }
            //check otp resend time 1 minute
            if ((DateTime.UtcNow - tempUser.OtpGeneratedAt).TotalMinutes < 1)
            {
                throw new OtpLimitExceededException();
            }

            //generate new otp
            var otp = _jwtService.random.Next(100000, 1000000);

            //send otp email
            await _mailServices.SendOtpEmail(tempUser.Email, tempUser.Username, otp.ToString());

            //update otp and otp generated time
            tempUser.Otp = otp;
            tempUser.OtpGeneratedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return;
        }
    }
}
