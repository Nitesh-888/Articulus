namespace Articulus.BLL.Exceptions.UserExceptions
{
    public class NotFollowingException : Exception
    {
        public NotFollowingException() : base("You are not following this user.")
        {
        }
    }
}
