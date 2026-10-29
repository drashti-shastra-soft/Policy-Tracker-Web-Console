using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace PolicyTrackerWebClient.Filter
{
    /// <summary>When the API token has expired (401), clear the session and go back to login.</summary>
    public class SessionExpiredFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var ex = context.Exception;
            while (ex != null && !(ex is UnauthorizedAccessException)) ex = ex.InnerException;
            if (ex == null) return;

            Infrastructure.HttpContextHelper.LoginUser = null;
            context.ExceptionHandled = true;

            if (context.HttpContext.Request.IsAjaxRequest())
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new JsonResult { Data = new { NotificationType = "error", NotificationMsg = Infrastructure.Constant.SessionExpired, Redirect = "/Authentication" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                return;
            }

            context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Authentication" }, { "action", "Index" } });
        }
    }
}
