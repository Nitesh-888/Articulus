using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NewsHub.Controllers.News
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        public NewsController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }
        //get all news
        [HttpGet]
        public async Task<IActionResult> GetAllNews()
        {
            var news = await _httpClientFactory.CreateClient("NewsApiClient")
                .GetAsync($"top-headlines?country=us&apiKey={_config["NewsApiOrg:Key"]}");

            if (news.IsSuccessStatusCode)
            {
                var content = await news.Content.ReadAsStringAsync();
                return Ok(content);
            }

            return BadRequest("Unable to retrieve news.");
        }
    }
}
