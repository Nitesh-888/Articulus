namespace Articulus.BLL.Exceptions
{
    public class ArticleNotFoundException : Exception
    {
        public ArticleNotFoundException(Guid articleId)
            : base($"Article with id '{articleId}' not found.") { }
    }
}
