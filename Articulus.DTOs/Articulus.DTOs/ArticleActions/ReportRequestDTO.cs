using Articulus.Data.Models;
namespace Articulus.DTOs.ArticleActions
{
    public class ReportRequestDTO
    {
        public required ReasonType Reason { get; set; }
        public required String Description { get; set; }
    }
}
