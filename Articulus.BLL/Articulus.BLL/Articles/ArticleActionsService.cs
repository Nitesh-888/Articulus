using Microsoft.EntityFrameworkCore;
using Articulus.BLL.Articles.Interfaces;
using Articulus.BLL.Exceptions;
using Articulus.Data;
using Articulus.Data.Models;

namespace Articulus.BLL.Articles
{
    public class ArticleActionsService : IArticleActionsService
    {
        private readonly AppDbContext _dbContext;
        public ArticleActionsService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task ReportArticleAsync(Guid userId, Guid articleId, ReasonType reason, string detail)
        {
            var article = await _dbContext.Articles
                .SingleOrDefaultAsync(a => a.ArticleId == articleId) ?? throw new ArticleNotFoundException(articleId);

            //check if the user has already reported this article
            var existingReport = await _dbContext.Reports
                .SingleOrDefaultAsync(r => r.ArticleId == articleId && r.UserId == userId);
            if (existingReport != null)
            {
                throw new AlreadyReportedException(userId, articleId);
            }

            var user = _dbContext.Users
                .SingleOrDefault(u => u.UserId == userId) ?? throw new UserNotFoundException(userId);
            var report = new Report
            {
                ArticleId = articleId,
                Article = article,
                UserId = userId,
                User = user,
                Reason = reason,
                Description = detail,
            };
            await _dbContext.Reports.AddAsync(report);
            await _dbContext.SaveChangesAsync();
        }

        public async Task BookmarkArticleAsync(Guid userId, Guid articleId)
        {
            var user = _dbContext.Users
                .SingleOrDefault(u => u.UserId == userId) ?? throw new UserNotFoundException(userId);


            var article = await _dbContext.Articles
                .SingleOrDefaultAsync(a => a.ArticleId == articleId) ?? throw new ArticleNotFoundException(articleId);
            
            var existingBookmark =  await _dbContext.Bookmarks
                .SingleOrDefaultAsync(b => b.ArticleId == articleId && b.UserId == userId);
            if (existingBookmark != null)
            {
                _dbContext.Bookmarks.Remove(existingBookmark);
                await _dbContext.SaveChangesAsync();
                return;
            }

            var bookmark = new Bookmark
            {
                ArticleId = articleId,
                Article = article,
                UserId = userId,
                User = user,
            };
            await _dbContext.Bookmarks.AddAsync(bookmark);
            await _dbContext.SaveChangesAsync();
        }
    }
}
