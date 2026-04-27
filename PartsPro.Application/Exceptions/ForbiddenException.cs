namespace PartsPro.Application.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "Access forbidden") 
            : base(message, 403)
        {
        }
    }
}
