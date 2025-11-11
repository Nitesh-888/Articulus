using Articulus.DTOs.Comments;

namespace Articulus.BLL.Articles.Interfaces
{
    public interface IArticleCommentsService
    {
        public Task<IEnumerable<GetAllCommentsDTO>> GetAllCommentsAsync(Guid articleId);
        public Task<GetCommentByIDResponseDTO> GetCommentByIdAsync(Guid userId, Guid articleId, Guid commentId);
        public Task AddCommentAsync(Guid userId, Guid articleId, string content);
        public Task EditCommentAsync(Guid userId, Guid articleId, Guid commentId, string newContent);
        public Task DeleteCommentAsync(Guid userId, Guid articleId, Guid commentId);

    }
}
