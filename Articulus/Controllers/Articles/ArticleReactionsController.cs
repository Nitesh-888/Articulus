using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Articulus.Data;
using Articulus.DTOs.Reactions;
using Articulus.DTOs.Users;
using Articulus.Filters;
using Articulus.Data.Models;
using System.Security.Claims;
using Articulus.BLL.Articles.Interfaces;
using Articulus.BLL.Exceptions;

namespace Articulus.Controllers.Articles
{
    [Route("api/Articles")]
    [ApiController]
    public class ArticleReactionsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IArticleReactionService _articleReactionService;

        public ArticleReactionsController(AppDbContext dbContext, IArticleReactionService articleReactionService)
        {
            _dbContext = dbContext;
            _articleReactionService = articleReactionService;
        }
        //Get all reactions on article
        [HttpGet("{ArticleId:guid}/Reactions")]
        public async Task<IActionResult> GetAllReactions(Guid ArticleId)
        {
            try
            {
                var result = await _articleReactionService.GetAllReactionsAsync(ArticleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => BadRequest("Article not found."),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
                };
            }
        }

        // Get reaction by user on article
        [HttpGet("{ArticleId:guid}/Reaction")]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> GetReactionByUser(Guid ArticleId, Guid UserId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _articleReactionService.GetReactionByUserAsync(userClaims.UserId, ArticleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => NotFound("Article not found."),
                    ReactionNotFoundException => NotFound("Reaction not found."),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
                };
            }
        }

        //user react to the article
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpPost("{articleId:guid}/Reaction")]
        public async Task<IActionResult> ReactToArticle(Guid articleId, [FromBody] CreateReactionRequestDTO reactionDto)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _articleReactionService.AddOrUpdateReactionAsync(userClaims.UserId, articleId, reactionDto.Type);
                return Ok("Article reaction added/updated successfully.");
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => NotFound("Article not found."),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
                };
            }
        }

        //remove reaction
        [HttpDelete("{articleId:guid}/Reaction")]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> RemoveReaction(Guid articleId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _articleReactionService.RemoveReactionAsync(userClaims.UserId, articleId);
                return Ok("Article reaction removed successfully.");
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => NotFound("Article not found."),
                    ReactionNotFoundException => NotFound("Reaction not found."),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
                };
            }
        }
    }
}
