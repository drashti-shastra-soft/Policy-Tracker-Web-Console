using PolicyTrackerWebClient.Filter;
using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    [Authentication]
    [AdminOnly]
    public class UserController : BaseController
    {
        private readonly IUserService _service;
        private readonly IDropdownService _dropdownService;

        public UserController(IUserService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        public ActionResult Index()
        {
            var mLister = new Domain.UserLister();
            mLister.Pagination.PageSize = 20;
            return View(mLister);
        }

        public ActionResult List(Domain.UserLister mLister)
        {
            try
            {
                mLister.SearchCriteria = mLister.SearchCriteria ?? new Domain.User();
                if (mLister.Pagination.PageSize == 0) mLister.Pagination.PageSize = 20;
                mLister.Pagination.Take = mLister.Pagination.PageSize > 0 ? mLister.Pagination.PageSize : 0;
                mLister = _service.GetAllAsync(mLister).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                ViewBag.Status = _dropdownService.GetStatusOptions();
                ViewBag.Pagination = _dropdownService.GetPageSizeOptions(mLister.Pagination.PageSize);
                ViewBag.Roles = _dropdownService.GetRoleOptions(mLister.SearchCriteria.RoleId > 0 ? mLister.SearchCriteria.RoleId : (int?)null);
            }
            return PartialView("_List", mLister);
        }

        [HttpGet]
        [AdminOnly]
        public ActionResult Create()
        {
            var m = new Domain.User();
            m.RoleId = (int)Enums.Role.Staff;
            LoadDropdowns(m);
            return PartialView("_Create", m);
        }

        [HttpPost]
        [AdminOnly]
        public ActionResult Create(Domain.User m)
        {
            try
            {
                m = Infrastructure.Helper.AuditHelper.ApplyAuditInfo(m);
                var saved = _service.CreateAsync(m).GetAwaiter().GetResult();
                var success = saved != null && saved.Id > 0;
                if (success) m.Id = saved.Id;
                m.Password = null;
                Notify(success, Enums.Operation.Save, Enums.MenuItem.User);
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                LoadDropdowns(m);
            }
            return PartialView("_Create", m);
        }

        [HttpGet]
        [AdminOnly]
        public ActionResult Get(int id)
        {
            var m = new Domain.User();
            try
            {
                m = _service.GetAsync(id).GetAwaiter().GetResult() ?? m;
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                LoadDropdowns(m);
            }
            return PartialView("_Create", m);
        }

        [HttpGet]
        [AdminOnly]
        public ActionResult Delete(int id)
        {
            try
            {
                _service.DeleteAsync(id).GetAwaiter().GetResult();
                return JsonNotify(true, string.Format(Constant.SuccessMessage, "deleted", "user"));
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        private void LoadDropdowns(Domain.User m)
        {
            ViewBag.Roles = _dropdownService.GetRoleOptions(m.RoleId > 0 ? m.RoleId : (int?)null);
        }
    }
}
