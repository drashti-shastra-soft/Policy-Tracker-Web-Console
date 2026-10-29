using PolicyTrackerWebClient.Infrastructure;
using System;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.IsAdmin = HttpContextHelper.IsAdmin;
            ViewBag.LoginUser = HttpContextHelper.LoginUser;
            base.OnActionExecuting(filterContext);
        }

        protected void Notify(bool success, Enums.Operation operation, Enums.MenuItem menuItem)
        {
            var op = EnumsHelper.GetEnumString(operation).StrToLower();
            var done = operation == Enums.Operation.Save ? "saved" : operation == Enums.Operation.Delete ? "deleted" : op + "ed";
            var item = EnumsHelper.GetEnumString(menuItem).StrToLower();

            ViewBag.NotificationType = success
                ? EnumsHelper.GetEnumString(Enums.Result.Success).StrToLower()
                : EnumsHelper.GetEnumString(Enums.Result.Error).StrToLower();

            ViewBag.NotificationMsg = success
                ? string.Format(Constant.SuccessMessage, done, item)
                : string.Format(Constant.ErrorMessage, op, item);
        }

        protected void NotifyError(Exception ex)
        {
            if (ex is UnauthorizedAccessException) throw ex;

            ViewBag.NotificationType = EnumsHelper.GetEnumString(Enums.Result.Error).StrToLower();
            ViewBag.NotificationMsg = ex is APIResponseException ? ex.Message : Constant.InternalServerError;
        }

        protected JsonResult JsonNotify(bool success, string message)
        {
            return Json(new
            {
                NotificationType = success ? "success" : "error",
                NotificationMsg = message
            }, JsonRequestBehavior.AllowGet);
        }

        protected static string ErrorText(Exception ex)
        {
            if (ex is UnauthorizedAccessException) throw ex;
            return ex is APIResponseException ? ex.Message : Constant.InternalServerError;
        }
    }
}
