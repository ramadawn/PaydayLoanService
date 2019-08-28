using System.Web.Mvc;
using TaxCash.LoanOrigination.Exceptions;

namespace TaxCash.LoanOrigination
{
	public class CustomExceptionFilterAttribute : FilterAttribute, IExceptionFilter
	{
		public void OnException(ExceptionContext filterContext)
		{
			if (filterContext.Exception is TaxCashSessionExpiredException)
			{
				filterContext.Result = new HttpUnauthorizedResult("Your session has expired. Please log in again.");
				filterContext.ExceptionHandled = true;
			}
		}
	}
}