using PolicyTrackerWebClient.Filter;
using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    [Authentication]
    public class PointsGridController : BaseController
    {
        private readonly IPointsGridService _service;
        private readonly IDropdownService _dropdownService;

        public PointsGridController(IPointsGridService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        public ActionResult Index()
        {
            var mLister = new Domain.PointsGridLister();
            mLister.Pagination.PageSize = 20;
            return View(mLister);
        }

        public ActionResult List(Domain.PointsGridLister mLister)
        {
            try
            {
                mLister.SearchCriteria = mLister.SearchCriteria ?? new Domain.PointsGrid();
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
                ViewBag.Brokers = _dropdownService.GetBrokerOptions(mLister.SearchCriteria.BrokerId > 0 ? mLister.SearchCriteria.BrokerId : HttpContextHelper.SelectedBrokerId);
                ViewBag.Categories = _dropdownService.GetCategoryOptions(mLister.SearchCriteria.CategoryId > 0 ? mLister.SearchCriteria.CategoryId : (int?)null);
            }
            return PartialView("_List", mLister);
        }

        [HttpGet]
        [AdminOnly]
        public ActionResult Create()
        {
            var m = new Domain.PointsGrid();
            m.BrokerId = HttpContextHelper.SelectedBrokerId ?? 0;
            m.AgeTo = 99;
            LoadDropdowns(m);
            return PartialView("_Create", m);
        }

        [HttpPost]
        [AdminOnly]
        public ActionResult Create(Domain.PointsGrid m)
        {
            try
            {
                m = Infrastructure.Helper.AuditHelper.ApplyAuditInfo(m);
                var saved = _service.CreateAsync(m).GetAwaiter().GetResult();
                var success = saved != null && saved.Id > 0;
                if (success) m.Id = saved.Id;
                Notify(success, Enums.Operation.Save, Enums.MenuItem.Points_Grid);
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
            var m = new Domain.PointsGrid();
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
                return JsonNotify(true, string.Format(Constant.SuccessMessage, "deleted", "points grid row"));
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        private void LoadDropdowns(Domain.PointsGrid m)
        {
            ViewBag.Brokers = _dropdownService.GetBrokerOptions(m.BrokerId > 0 ? m.BrokerId : (int?)null);
            ViewBag.Categories = _dropdownService.GetCategoryOptions(m.CategoryId > 0 ? m.CategoryId : (int?)null);
        }
    }
}
