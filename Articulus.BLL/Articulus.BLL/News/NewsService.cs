using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.Net.Http;
using Articulus.BLL.News.DTOs;
using Articulus.BLL.News.Interfaces;
using System.Text.Json;

namespace Articulus.BLL.News
{
    public class NewsService : INewsService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        public NewsService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<GetAllNewsDTO> GetAllNewsAsync()
        {
            var news = await _httpClientFactory.CreateClient("NewsApiClient")
                .GetAsync($"top-headlines?country=us&apiKey={_config["NewsApiOrg:Key"]}");

            if (news.IsSuccessStatusCode)
            {
                var content = await news.Content.ReadAsStringAsync();
                if (content == null)
                {
                    return new GetAllNewsDTO
                    {
                        Result = NewsFetchResult.NotFound
                    };
                }

                var allNews = JsonSerializer.Deserialize<ApiNewsDTO>(content);
                if (allNews == null || allNews.Articles == null || allNews.Articles.Count == 0)
                {
                    return new GetAllNewsDTO
                    {
                        Result = NewsFetchResult.NotFound
                    };
                }

                //mapping ApiNewsDTO to GetAllNewsDTO
                return new GetAllNewsDTO
                {
                    Result = NewsFetchResult.Success,
                    AllNews = allNews.Articles.Select(a => new GetAllNewsResponseDTO
                    {
                        SourceName = a.Source.Name,
                        Title = a.Title,
                        Description = a.Description,
                        Url = a.Url,
                        PublishedAt = a.PublishedAt,
                        UrlToImage = a.UrlToImage,
                        Author = a.Author,
                        Content = a.Content
                    }).ToList()
                };
            }
            else
            {
                return new GetAllNewsDTO
                {
                    Result = NewsFetchResult.Failure
                };
            }
        }
    }
}
