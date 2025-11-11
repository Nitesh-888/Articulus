namespace Articulus.BLL.Exceptions.UserExceptions
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException(string email) : base($"The email address '{email}' is already associated with another account.")
        {
        }
    }
}
