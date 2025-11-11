namespace Articulus.BLL.Exceptions.UserExceptions
{
    public class CannotFollowSelfException : Exception
    {
        public CannotFollowSelfException() : base("You cannot follow yourself.")
        {
        }
    }
}
