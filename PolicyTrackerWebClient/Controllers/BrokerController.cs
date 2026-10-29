using PolicyTrackerWebClient.Filter;
using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    [Authentication]
    public class BrokerController : BaseController
    {
        private readonly IBrokerService _service;
        private readonly IDropdownService _dropdownService;

        public BrokerController(IBrokerService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        public ActionResult Index()
        {
            var mLister = new Domain.BrokerLister();
            mLister.Pagination.PageSize = 20;
            return View(mLister);
        }

        public ActionResult List(Domain.BrokerLister mLister)
        {
            try
            {
                mLister.SearchCriteria = mLister.SearchCriteria ?? new Domain.Broker();
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
            }
            return PartialView("_List", mLister);
        }

        [HttpGet]
        [AdminOnly]
        public ActionResult Create()
        {
            var m = new Domain.Broker();
            m.TdsRate = 5;
            LoadDropdowns(m);
            return PartialView("_Create", m);
        }

        [HttpPost]
        [AdminOnly]
        public ActionResult Create(Domain.Broker m)
        {
            try
            {
                m = Infrastructure.Helper.AuditHelper.ApplyAuditInfo(m);
                var saved = _service.CreateAsync(m).GetAwaiter().GetResult();
                var success = saved != null && saved.Id > 0;
                if (success) m.Id = saved.Id;
                Notify(success, Enums.Operation.Save, Enums.MenuItem.Broker);
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
            var m = new Domain.Broker();
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
                return JsonNotify(true, string.Format(Constant.SuccessMessage, "deleted", "broker"));
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        private void LoadDropdowns(Domain.Broker m)
        {
        }
    }
}
