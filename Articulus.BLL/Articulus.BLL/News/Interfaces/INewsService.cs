using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Articulus.BLL.News.DTOs;

namespace Articulus.BLL.News.Interfaces
{
    public enum NewsFetchResult
    {
        Success,
        Failure,
        NotFound
    }
    public interface INewsService
    {
        public Task<GetAllNewsDTO> GetAllNewsAsync();
    }
}
