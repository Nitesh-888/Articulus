using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Articulus.Data;
using Articulus.DTOs.Comments;
using Articulus.DTOs.Users;
using Articulus.Filters;
using Articulus.Data.Models;
using System.Security.Claims;
using Articulus.BLL.Articles.Interfaces;
using Articulus.BLL.Exceptions;

namespace Articulus.Controllers.Articles
{
    [Route("api/Articles/{ArticleId:guid}/Comments")]
    [ApiController]
    public class ArticleCommentsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IArticleCommentsService _articleCommentsService;
        public ArticleCommentsController(AppDbContext dbContext, IArticleCommentsService articleCommentsService)
        {
            _dbContext = dbContext;
            _articleCommentsService = articleCommentsService;
        }
        //Get all Comments on article
        [HttpGet]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ActionResult<IEnumerable<GetAllCommentsDTO>>> GetAllComment(Guid ArticleId)
        {
            IEnumerable<GetAllCommentsDTO> result;
            try
            {
                result = await _articleCommentsService.GetAllCommentsAsync(ArticleId);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => NotFound("Article not found."),
                    _ => BadRequest("An error occurred while processing your request.")
                };
            }
            return Ok(result);
        }

        //Get comment by id
        [HttpGet("{CommentId:guid}")]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> GetCommentById(Guid ArticleId, Guid CommentId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                var comment = await _articleCommentsService.GetCommentByIdAsync(userClaims.UserId, ArticleId, CommentId);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => Unauthorized(),
                    CommentNotFoundException => NotFound("Comment not found for the specified article."),
                    ArticleNotFoundException => NotFound("Article not found."),
                    _ => BadRequest("An error occurred while processing your request.")
                };
            }
        }

        //Create comment on article
        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> CreateComment(Guid ArticleId, [FromBody] CreateCommentRequestDTO CommentDto)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _articleCommentsService.AddCommentAsync(userClaims.UserId, ArticleId, CommentDto.Text);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => Unauthorized(),
                    ArticleNotFoundException => NotFound("Article not found."),
                    _ => BadRequest("An error occurred while processing your request.")
                };
            }
            return CreatedAtAction(nameof(GetCommentById), new { ArticleId = ArticleId }, null);
        }

        //Update comment
        [HttpPut("{CommentId:guid}")]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> UpdateComment(Guid ArticleId, Guid CommentId, [FromBody] UpdateCommentRequestDTO CommentDto)
        {
            if (CommentDto == null)
            {
                return BadRequest();
            }

            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }
            try
            {
                await _articleCommentsService.EditCommentAsync(userClaims.UserId, ArticleId, CommentId, CommentDto.Text);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => Unauthorized(),
                    CommentNotFoundException => NotFound("Comment not found for the specified article."),
                    ArticleNotFoundException => NotFound("Article not found."),
                    ForbiddenException => Forbid(),
                    _ => BadRequest("An error occurred while processing your request.")
                };
            }
            return NoContent();
        }

        //Delete comment
        [HttpDelete("{CommentId:guid}")]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> DeleteComment(Guid ArticleId, Guid CommentId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _articleCommentsService.DeleteCommentAsync(userClaims.UserId, ArticleId, CommentId);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => Unauthorized(),
                    CommentNotFoundException => NotFound("Comment not found for the specified article."),
                    ArticleNotFoundException => NotFound("Article not found."),
                    ForbiddenException => Forbid(),
                    _ => BadRequest("An error occurred while processing your request.")
                };
            }
            return NoContent();
        }
    }
}
