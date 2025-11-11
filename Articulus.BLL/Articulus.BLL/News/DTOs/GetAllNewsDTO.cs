using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Articulus.BLL.News.Interfaces;

namespace Articulus.BLL.News.DTOs
{
    public class GetAllNewsDTO
    {
        public IEnumerable<GetAllNewsResponseDTO>? AllNews { get; set; }
        public required NewsFetchResult Result { get; set; }
    }
}
