using Articulus.DTOs.Reactions;
using Articulus.Data.Models;

namespace Articulus.BLL.Articles.Interfaces
{
    public interface IArticleReactionService
    {
        public Task<IEnumerable<GetAllReactionsResponseDTO>> GetAllReactionsAsync(Guid articleId);
        public Task<ReactionResponseDTO> GetReactionByUserAsync(Guid userId, Guid articleId);
        public Task AddOrUpdateReactionAsync(Guid userId, Guid articleId, ReactionType reactionType);
        public Task RemoveReactionAsync(Guid userId, Guid articleId);
    }
}
