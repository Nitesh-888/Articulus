using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Articulus.DTOs.ArticleActions;
using Articulus.DTOs.Users;
using Articulus.Filters;
using Articulus.Data.Models;
using Articulus.BLL.Articles.Interfaces;
using Articulus.BLL.Exceptions;

namespace Articulus.Controllers.Articles
{
    [Route("api/Articles/{ArticleId:guid}")]
    [ApiController]
    public class ArticleActionsController : ControllerBase
    {
        private readonly Data.AppDbContext _dbContext;
        private readonly IArticleActionsService _articleActionsService;
        public ArticleActionsController(Data.AppDbContext dbContext, IArticleActionsService articleActionsService)
        {
            _dbContext = dbContext;
            _articleActionsService = articleActionsService;
        }
        //Report an article
        [HttpPost("Report")]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> ReportArticle(Guid ArticleId, [FromBody] ReportRequestDTO request)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _articleActionsService.ReportArticleAsync(userClaims.UserId, ArticleId, request.Reason, request.Description);
                
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => NotFound("Article not found"),
                    UserNotFoundException => Unauthorized(),
                    AlreadyReportedException => BadRequest("You have already reported this article"),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while reporting the article")
                };
            }
            return Ok("Article reported successfully");
        }

        //bookmark an article
        [HttpPost("Bookmark")]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> BookmarkArticle(Guid ArticleId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _articleActionsService.BookmarkArticleAsync(userClaims.UserId, ArticleId);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => NotFound("Article not found"),
                    UserNotFoundException => Unauthorized(),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while bookmarking the article")
                };
            }
            return Ok("Article bookmark toggled successfully");
        }
    }
}
