using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Articulus.BLL.News.DTOs
{
    public class ApiNewsDTO
    {
        public string? Status { get; set; }
        public int TotalResults { get; set; }
        public List<NewsArticles>? Articles { get; set; }
    }
}
