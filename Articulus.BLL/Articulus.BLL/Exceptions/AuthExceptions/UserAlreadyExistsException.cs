namespace Articulus.BLL.Exceptions.Auth_Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string emailOrUsername) : base($"A user with the email or username '{emailOrUsername}' already exists.")
        {
        }
    }
}
