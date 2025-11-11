namespace Articulus.BLL.Exceptions
{
    public class CommentNotFoundException : Exception
    {
        public CommentNotFoundException(Guid commentId)
            : base($"Comment with id '{commentId}' not found.") { }
    }
}
