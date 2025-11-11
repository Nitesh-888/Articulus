namespace Articulus.BLL.Exceptions.Auth_Exceptions
{
    public class OtpExpiredException : Exception
    {
        public OtpExpiredException() : base("The provided OTP has expired. Please request a new one.")
        {
        }
    }
}
