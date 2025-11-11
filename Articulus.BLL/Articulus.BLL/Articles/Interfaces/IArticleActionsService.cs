using Articulus.Data.Models;

namespace Articulus.BLL.Articles.Interfaces
{
    public interface IArticleActionsService
    {
        public Task ReportArticleAsync(Guid userId, Guid articleId, ReasonType reason, string detail);
        public Task BookmarkArticleAsync(Guid userId, Guid articleId);
    }
}

