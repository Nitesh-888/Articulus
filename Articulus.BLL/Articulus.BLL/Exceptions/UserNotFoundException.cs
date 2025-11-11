namespace Articulus.BLL.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid userId)
            : base($"User with id '{userId}' not found.") { }

        public UserNotFoundException(string email)
            : base($"User with email '{email}' not found.") { }
    }
}
