namespace Articulus.BLL.Exceptions.UserExceptions
{
    public class UsernameAlreadyExistsException : Exception
    {
        public UsernameAlreadyExistsException(string username) : base($"The username '{username}' is already taken.")
        {
        }
    }
}
