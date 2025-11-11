using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Articulus.Data;
using Articulus.DTOs.Users;
using Articulus.Filters;
using Articulus.Data.Models;
using Articulus.BLL.Articles.Interfaces;
using Articulus.BLL.Exceptions;

namespace Articulus.Controllers.Articles
{
    [Route("api/Articles/{ArticleId}/Comments/{CommentId}/Votes")]
    [ApiController]
    public class ArticleCommentVotesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IArticleCommentVoteService _articleCommentVoteService;

        public ArticleCommentVotesController(AppDbContext dbContext, IArticleCommentVoteService articleCommentVoteService)
        {
            _dbContext = dbContext;
            _articleCommentVoteService = articleCommentVoteService;
        }

        //UpVote for a comment
        [HttpPost]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> UpVoteForComment(Guid ArticleId, Guid CommentId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }
            try
            {
                await _articleCommentVoteService.UpvoteAsync(userClaims.UserId, ArticleId, CommentId);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    VoteNotFoundException => NotFound("Comment not found for the specified article."),
                    _ => BadRequest("An error occurred while processing your request.")
                };
            }
            return Ok("Vote toggled/added/removed successfully.");
        }

        //DownVote for a comment
        [HttpDelete]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> DownVoteForComment(Guid ArticleId, Guid CommentId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            var comment = await _dbContext.Comments.FindAsync(CommentId);
            if (comment == null || comment.ArticleId != ArticleId)
            {
                return NotFound("Comment not found for the specified article.");
            }

            try
            {
                await _articleCommentVoteService.DownvoteAsync(userClaims.UserId, ArticleId, CommentId);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    VoteNotFoundException => NotFound("Comment not found for the specified article."),
                    _ => BadRequest("An error occurred while processing your request.")
                };
            }
            return Ok("Vote toggled/added/removed successfully.");
        }
    }
}
