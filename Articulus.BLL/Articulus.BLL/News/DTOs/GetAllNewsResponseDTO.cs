using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Articulus.BLL.News.DTOs
{
    public class GetAllNewsResponseDTO
    {
        public required string SourceName { get; set; }
        public required string Author { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
        public required string UrlToImage { get; set; }
        public required DateTime PublishedAt { get; set; }
        public required string Content { get; set; }
    }
}
