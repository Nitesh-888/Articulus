namespace Articulus.BLL.Exceptions
{
    public class AlreadyReportedException : Exception
    {
        public AlreadyReportedException(Guid userId, Guid articleId)
            : base($"User '{userId}' already reported article '{articleId}'.") { }
    }
}
