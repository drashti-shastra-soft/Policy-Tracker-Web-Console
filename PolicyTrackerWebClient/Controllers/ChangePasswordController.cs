using PolicyTrackerWebClient.Filter;
using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    [Authentication]
    public class ChangePasswordController : BaseController
    {
        private readonly IUserService _service;

        public ChangePasswordController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new Domain.ChangePassword());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Domain.ChangePassword mChangePassword)
        {
            try
            {
                if (mChangePassword.NewPassword.IsNullOrEmpty() || mChangePassword.NewPassword.Length < 8)
                    throw new APIResponseException("The new password must be at least 8 characters.");
                if (mChangePassword.NewPassword != mChangePassword.ConfirmPassword)
                    throw new APIResponseException("The new password and confirmation do not match.");

                _service.ChangePasswordAsync(mChangePassword).GetAwaiter().GetResult();
                Notify(true, Enums.Operation.Save, Enums.MenuItem.Password);
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            return View(new Domain.ChangePassword());
        }
    }
}
