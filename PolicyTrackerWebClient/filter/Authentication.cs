using System.Web.Mvc;
using System.Web.Routing;

namespace PolicyTrackerWebClient.Filter
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var loginUser = Infrastructure.HttpContextHelper.LoginUser;

            if (loginUser == null)
            {
                if (context.HttpContext.Request.IsAjaxRequest())
                {
                    context.HttpContext.Response.StatusCode = 401;
                    context.Result = new JsonResult { Data = new { NotificationType = "error", NotificationMsg = Infrastructure.Constant.SessionExpired, Redirect = "/Authentication" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    return;
                }

                var returnUrl = context.HttpContext?.Request?.RawUrl ?? "/";

                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Authentication" },
                    { "action", "Index" },
                    { "returnUrl", returnUrl }
                });
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>Only administrators can open the action.</summary>
    public class AdminOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Infrastructure.HttpContextHelper.IsAdmin)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Dashboard" }, { "action", "Index" } });
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}
