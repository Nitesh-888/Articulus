using Articulus.DTOs.Articles;


namespace Articulus.BLL.Articles.Interfaces
{
    public interface IArticleService
    {
        public Task<IEnumerable<GetAllArticlesResponseDTO>> GetAllArticlesAsync();
        public Task<GetArticleResponseDTO> GetArticleByIdAsync(Guid userId, Guid articleId);
        public Task<GetArticleResponseDTO> CreateArticleAsync(Guid userId, CreateArticleRequestDTO articleDto);
        public Task UpdateArticleAsync(Guid userId, Guid articleId, UpdateArticleRequestDTO articleDto);
        public Task DeleteArticleAsync(Guid userId, Guid articleId);
    }
}
