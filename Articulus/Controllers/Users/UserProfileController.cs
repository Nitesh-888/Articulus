using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Articulus.Data;
using Articulus.DTOs.Users;
using Articulus.Filters;
using Articulus.BLL.Users.Interfaces;
using Articulus.BLL.Exceptions;
using Articulus.BLL.Exceptions.UserExceptions;

namespace Articulus.Controllers.Users
{
    [Route("api/User")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(userClaims.UserId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => BadRequest("User not found"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the profile.")
                };
            }
        }

        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpPut("Profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDTO updatedInfo)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _userProfileService.UpdateUserProfileAsync(userClaims.UserId, updatedInfo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => BadRequest("User not found"),
                    EmailAlreadyExistsException => BadRequest("Email already exists"),
                    UsernameAlreadyExistsException => BadRequest("Username already exists"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the profile.")
                };
            }
        }
    }
}
