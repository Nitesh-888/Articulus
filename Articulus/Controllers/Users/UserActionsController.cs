using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Articulus.Data;
using System.Security.Claims;
using Articulus.DTOs.Users;
using Articulus.Data.Models;
using Articulus.Filters;
using Articulus.BLL.Users.Interfaces;
using Articulus.BLL.Exceptions;
using Articulus.BLL.Exceptions.UserExceptions;

namespace Articulus.Controllers.Users
{
    [Route("api/User")]
    [ApiController]
    public class UserActionsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserActionsService _userActionsService;

        public UserActionsController(AppDbContext dbContext, IUserActionsService userActionsService)
        {
            _dbContext = dbContext;
            _userActionsService = userActionsService;
        }

        //Feedback
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpPost("Feedback")]
        public async Task<IActionResult> GiveFeedback([FromBody] FeedbackDTO feedback)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }
            try
            {
                await _userActionsService.GiveFeedbackAsync(userClaims.UserId, feedback.Text, feedback.Rating);
                return Ok("Feedback submitted successfully.");
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => BadRequest("User not found."),
                    _ => BadRequest()
                };
            }
        }

        //Delete User
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _userActionsService.DeleteUserAsync(userClaims.UserId);
                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => BadRequest("User not found."),
                    _ => BadRequest()
                };
            }
        }

        //follow another user
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpPost("Follow/{followeeId}")]
        public async Task<IActionResult> FollowUser(Guid followeeId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _userActionsService.FollowUserAsync(userClaims.UserId, followeeId);
                return Ok("Followed successfully.");
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => NotFound("User or Followee to follow not found."),
                    AlreadyFollowingException => BadRequest("You are already following this user."),
                    CannotFollowSelfException => BadRequest("You cannot follow yourself."),
                    _ => BadRequest()
                };
            }
        }

        //unfollow another user
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpPost("Unfollow/{followeeId}")]
        public async Task<IActionResult> UnfollowUser(Guid followeeId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _userActionsService.UnfollowUserAsync(userClaims.UserId, followeeId);
                return Ok("Unfollowed successfully.");
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    NotFollowingException => BadRequest("You are not following this user."),
                    _ => BadRequest()
                };
            }
        }
    }
}
