namespace Articulus.BLL.Exceptions
{
    public class ReactionNotFoundException : Exception
    {
        public ReactionNotFoundException(Guid userId, Guid articleId) : base($"Reaction not found for user {userId} and article {articleId}.")
        {
        }
    }
}
