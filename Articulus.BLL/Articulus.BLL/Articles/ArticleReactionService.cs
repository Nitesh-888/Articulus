using Articulus.BLL.Articles.Interfaces;
using Articulus.Data;
using Microsoft.EntityFrameworkCore;
using Articulus.DTOs.Reactions;
using Articulus.Data.Models;
using Articulus.BLL.Exceptions;

namespace Articulus.BLL.Articles
{
    public class ArticleReactionService : IArticleReactionService
    {
        private readonly AppDbContext _dbContext;
        public ArticleReactionService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<GetAllReactionsResponseDTO>> GetAllReactionsAsync(Guid articleId)
        {
            //check if article exist or not
            var article = await _dbContext.Articles.FindAsync(articleId);
            if (article == null)
            {
                throw new ArticleNotFoundException(articleId);
            }

            var reactions = await _dbContext.Reactions
                .Where(r => r.ArticleId == articleId)
                .Select(r => new GetAllReactionsResponseDTO
                {
                    UserId = r.UserId,
                    Type = r.Type,
                    ReactedAt = r.CreatedAt
                })
                .ToListAsync();

            return reactions;
        }

        public async Task AddOrUpdateReactionAsync(Guid userId, Guid articleId, ReactionType reactionType)
        {
            // extract article from db
            var article = await _dbContext.Articles
                .Include(a => a.Reactions)
                .SingleOrDefaultAsync(a => a.ArticleId == articleId) ?? throw new ArticleNotFoundException(articleId);

            // Check if article is already reacted by the user
            var existingReaction = article.Reactions.SingleOrDefault(r => r.UserId == userId);
            if (existingReaction != null)
            {
                existingReaction.Type = reactionType;
                _dbContext.Reactions.Update(existingReaction);
                await _dbContext.SaveChangesAsync();
                return;
            }

            var reaction = new Reaction
            {
                ArticleId = articleId,
                UserId = userId,
                Type = reactionType
            };

            await _dbContext.Reactions.AddAsync(reaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ReactionResponseDTO> GetReactionByUserAsync(Guid userId, Guid articleId)
        {
            var reaction = await _dbContext.Reactions
                .SingleOrDefaultAsync(r => r.ArticleId == articleId && r.UserId == userId) ?? throw new ReactionNotFoundException(userId, articleId);
            var response = new ReactionResponseDTO
            {
                UserId = reaction.UserId,
                ArticleId = reaction.ArticleId,
                Type = reaction.Type,
                ReactedAt = reaction.CreatedAt
            };
            return response;
        }

        public async Task RemoveReactionAsync(Guid userId, Guid articleId)
        {
            var article = await _dbContext.Articles.FindAsync(articleId) ?? throw new ArticleNotFoundException(articleId);
            var existingReaction = await _dbContext.Reactions.SingleOrDefaultAsync(r => r.ArticleId == articleId && r.UserId == userId) ?? throw new ReactionNotFoundException(userId, articleId);
            _dbContext.Reactions.Remove(existingReaction);
            await _dbContext.SaveChangesAsync();
        }
    }
}
