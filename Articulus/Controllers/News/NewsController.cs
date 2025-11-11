using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Articulus.BLL.News.Interfaces;

namespace Articulus.Controllers.News
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }
        //get all news
        [HttpGet]
        public async Task<IActionResult> GetAllNews()
        {
            var Result = await _newsService.GetAllNewsAsync();
            return Result.Result switch
            {
                NewsFetchResult.Success => Ok(Result.AllNews),
                NewsFetchResult.NotFound => NotFound("No news articles found."),
                NewsFetchResult.Failure => StatusCode(500, "An error occurred while fetching news."),
                _ => StatusCode(500, "An unexpected error occurred.")
            };
        }
    }
}
