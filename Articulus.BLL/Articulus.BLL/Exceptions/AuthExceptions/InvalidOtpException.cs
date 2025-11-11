namespace Articulus.BLL.Exceptions.Auth_Exceptions
{
    public class InvalidOtpException : Exception
    {
        public InvalidOtpException() : base("The provided OTP is invalid or has expired.")
        {
        }
    }
}
