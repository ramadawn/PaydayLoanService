using System;
using System.IdentityModel.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TaxCash.LoanOrigination.Exceptions;

namespace TaxCash.LoanOrigination
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		void Application_Error(object sender, EventArgs e)
		{
			Exception exception = Server.GetLastError();
			if (exception != null)
			{
				if (exception is TaxCashSessionExpiredException)
				{
					HttpContext.Current.Response.Redirect("~/ErrorHandler/SessionExpired");
				}
			}
		}
	}
}
