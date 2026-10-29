using PolicyTrackerWebClient.Infrastructure;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    public class LogoutController : Controller
    {
        // GET: Logout
        public ActionResult Index()
        {
            HttpContextHelper.LoginUser = null;
            Session.Abandon();
            return RedirectToAction("Index", "Authentication");
        }
    }
}
