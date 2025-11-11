using Articulus.BLL.Articles.Interfaces;
using Articulus.BLL.Exceptions;
using Articulus.Data;
using Articulus.Data.Models;
using Articulus.DTOs.Articles;
using Microsoft.EntityFrameworkCore;

namespace Articulus.BLL.Articles
{
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _dbContext;
        public ArticleService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<GetAllArticlesResponseDTO>> GetAllArticlesAsync()
        {
            var articlesList = await _dbContext.Articles.Select(a => new GetAllArticlesResponseDTO
            {
                ArticleId = a.ArticleId,
                Title = a.Title,
                CoverImageUrl = a.CoverImageUrl,
                ViewsCount = a.Views.Count,
                ReactionsCount = a.Reactions.Count,
                CommentsCount = a.Comments.Count,
                CreatedAt = a.CreatedAt,
            }).ToListAsync();

            return articlesList;
        }

        public async Task<GetArticleResponseDTO> GetArticleByIdAsync(Guid userId, Guid articleId)
        {
            //extract the article from db
            var article = await _dbContext.Articles
                .Include(a => a.Reactions)
                .Include(a => a.Comments)
                .Include(a => a.Views)
                .SingleOrDefaultAsync(a => a.ArticleId == articleId);

            if (article == null)
            {
                throw new ArticleNotFoundException(articleId);
            }

            // extracting author from db
            var author = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.UserId == article.UserId);
            if (author == null)
            {
                throw new UserNotFoundException(article.UserId);
            }

            // Record the view
            // Record a new view if the user is not the author and has not viewed the article before.
            bool isAuthor = article.UserId == userId;
            bool hasAlreadyViewed = article.Views.Any(v => v.UserId == userId);
            if (!isAuthor && !hasAlreadyViewed)
            {
                var view = new View
                {
                    ArticleId = article.ArticleId,
                    UserId = userId,
                };
                await _dbContext.Views.AddAsync(view);
                await _dbContext.SaveChangesAsync();
            }


            var response = new GetArticleResponseDTO
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                CoverImageUrl = article.CoverImageUrl,
                Content = article.Content,
                AuthorId = article.UserId,
                AuthorName = author.FirstName + " " + author.LastName,
                AuthorProfileImageUrl = author.ProfileImageUrl,

                ViewsCount = article.Views.Count,
                ReactionsCount = article.Reactions.Count,
                CommentsCount = article.Comments.Count,
                CreatedAt = article.CreatedAt,
                LastUpdatedAt = article.LastUpdatedAt
            };
            return response;
        }

        public async Task<GetArticleResponseDTO> CreateArticleAsync(Guid userId, CreateArticleRequestDTO articleDto)
        {
            User? user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            var article = new Article
            {
                Title = articleDto.Title,
                Content = articleDto.Content,
                CoverImageUrl = articleDto.CoverImageUrl,
                UserId = userId,
                User = user
            };

            await _dbContext.Articles.AddAsync(article);
            await _dbContext.SaveChangesAsync();

            var response = new GetArticleResponseDTO
            {
                ArticleId = article.ArticleId,
                Title = article.Title,
                CoverImageUrl = article.CoverImageUrl,
                Content = article.Content,
                AuthorId = article.UserId,
                AuthorName = user.FirstName + " " + user.LastName,
                AuthorProfileImageUrl = user.ProfileImageUrl,
                ViewsCount = article.Views.Count,
                ReactionsCount = article.Reactions.Count,
                CommentsCount = article.Comments.Count,
                CreatedAt = article.CreatedAt,
                LastUpdatedAt = article.LastUpdatedAt
            };

            return response;
        }

        public async Task UpdateArticleAsync(Guid userId, Guid articleId, UpdateArticleRequestDTO articleDto)
        {
            var user = await _dbContext.Users
                .Include(u => u.Articles)
                .SingleOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            // Check if the article belongs to the user
            if (!user.Articles.Any(a => a.ArticleId == articleId))
            {
                throw new UserNotFoundException(userId);
            }

            var article = await _dbContext.Articles.FindAsync(articleId);
            if (article == null)
            {
                throw new ArticleNotFoundException(articleId);
            }

            article.Title = articleDto.Title ?? article.Title;
            article.Content = articleDto.Content ?? article.Content;
            article.CoverImageUrl = articleDto.CoverImageUrl ?? article.CoverImageUrl;
            article.LastUpdatedAt = DateTime.UtcNow;

            _dbContext.Articles.Update(article);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteArticleAsync(Guid userId, Guid articleId)
        {
            var user = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            var article = await _dbContext.Articles.FindAsync(articleId);
            if (article == null)
            {
                throw new ArticleNotFoundException(articleId);
            }

            // Check if the article belongs to the user
            if (article.UserId != userId)
            {
                throw new UserNotFoundException(userId);
            }


            _dbContext.Articles.Remove(article);
            await _dbContext.SaveChangesAsync();
        }
    }
}
