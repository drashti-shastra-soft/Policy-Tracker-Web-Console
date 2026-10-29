using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    public class AuthenticationController : Controller
    {
        private const string CookieName = "PolicyTrackerEmail";
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var mCredential = new Domain.Credential();

            // Only the email is remembered – never the password.
            var cookie = Request.Cookies[CookieName];
            if (cookie != null && cookie.Value.IsNotNullOrEmpty())
            {
                mCredential.EmailAddress = HttpUtility.UrlDecode(cookie.Value);
                mCredential.RememberMe = true;
            }

            return View(mCredential);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Domain.Credential mCredential)
        {
            try
            {
                string returnUrl = Request.Params["returnUrl"];
                var mUser = await _service.AuthenticateAsync(mCredential);

                if (mUser == null || mUser.Id <= 0)
                {
                    ViewBag.ErrorMessage = "Authentication failed: user identity could not be verified!";
                    return View(mCredential);
                }

                Session.Clear();
                HttpContextHelper.LoginUser = mUser;

                var cookie = new HttpCookie(CookieName, mCredential.RememberMe ? HttpUtility.UrlEncode(mCredential.EmailAddress) : string.Empty)
                {
                    Expires = mCredential.RememberMe ? DateTime.Now.AddYears(1) : DateTime.Now.AddDays(-1),
                    HttpOnly = true,
                };
                HttpContext.Response.Cookies.Add(cookie);

                // only local redirects
                if (returnUrl.IsNotNullOrEmpty() && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex is APIResponseException ? ex.Message : "Unable to reach the server. Please try again.";
                mCredential.Password = null;
                return View(mCredential);
            }
        }
    }
}
