namespace Articulus.BLL.Exceptions.Auth_Exceptions
{
    public class WeakPasswordException : Exception
    {
        public WeakPasswordException() : base("The provided password is too weak.")
        {
        }
    }
}
