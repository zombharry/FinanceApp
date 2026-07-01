using System.Net;

namespace FinanceApp.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string resourceName, object key )
            : base($"{resourceName} with identifier '{key}' was not found.", HttpStatusCode.BadRequest)
        {
        }
    }
}
