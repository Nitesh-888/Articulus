namespace Articulus.BLL.Exceptions.Auth_Exceptions
{
    public class OtpLimitExceededException : Exception
    {
        public OtpLimitExceededException() : base("The maximum number of OTP requests has been exceeded. Please try again later.")
        {
        }
    }
}
