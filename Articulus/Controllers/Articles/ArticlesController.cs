using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Articulus.Data;
using Articulus.DTOs.Articles;
using Articulus.DTOs.Users;
using Articulus.Filters;
using Articulus.Data.Models;
using System.Security.Claims;
using Articulus.BLL.Articles.Interfaces;
using Articulus.BLL.Exceptions;

namespace Articulus.Controllers.Articles
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IArticleService _articleService;
        public ArticlesController(AppDbContext dbContext, IArticleService articleService)
        {
            _dbContext = dbContext;
            _articleService = articleService;
        }

        //Get all articles
        [HttpGet]
        public async Task<IActionResult> GetALLArticles()
        {
            return Ok(await _articleService.GetAllArticlesAsync());
        }

        //Get article by id
        [HttpGet("{ArticleId:guid}")]
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> GetArticleById(Guid ArticleId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _articleService.GetArticleByIdAsync(userClaims.UserId, ArticleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => NotFound(),
                    UserNotFoundException => Unauthorized(),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }
        }

        //Create article
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequestDTO ArticleDto)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _articleService.CreateArticleAsync(userClaims.UserId, ArticleDto);
                return CreatedAtAction(nameof(GetArticleById), new { ArticleId = result.ArticleId }, result);
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    UserNotFoundException => Unauthorized(),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }
        }

        //Update article
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpPut("{articleId:guid}")]
        public async Task<IActionResult> UpdateArticle(Guid articleId, [FromBody] UpdateArticleRequestDTO ArticleDto)
        {
            if (ArticleDto == null)
            {
                return BadRequest();
            }
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _articleService.UpdateArticleAsync(userClaims.UserId, articleId, ArticleDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => NotFound(),
                    UserNotFoundException => Unauthorized(),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }
        }

        //Delete article
        [ServiceFilter(typeof(CustomAuthorizeFilter))]
        [HttpDelete("{articleId:guid}")]
        public async Task<IActionResult> DeleteArticle(Guid articleId)
        {
            if (HttpContext.Items["UserJwtClaims"] is not UserJwtClaims userClaims)
            {
                return Unauthorized();
            }

            try
            {
                await _articleService.DeleteArticleAsync(userClaims.UserId, articleId);
                return NoContent();
            }
           catch (Exception ex)
            {
                return ex switch
                {
                    ArticleNotFoundException => NotFound(),
                    UserNotFoundException => Unauthorized(),
                    _ => StatusCode(StatusCodes.Status500InternalServerError)
                };
            }
        }
    }
}
