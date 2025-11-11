namespace Articulus.BLL.Exceptions.AuthExceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("The provided credentials are invalid.")
        {
        }
    }
}
