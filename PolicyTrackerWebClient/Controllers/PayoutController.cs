using PolicyTrackerWebClient.Filter;
using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    [Authentication]
    public class PayoutController : BaseController
    {
        private readonly IPayoutService _service;
        private readonly IDropdownService _dropdownService;

        public PayoutController(IPayoutService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        public ActionResult Index()
        {
            var mLister = new Domain.PayoutLister();
            mLister.SearchCriteria.BrokerId = HttpContextHelper.SelectedBrokerId ?? 0;
            mLister.Pagination.PageSize = -1;
            return View(mLister);
        }

        public ActionResult List(Domain.PayoutLister mLister)
        {
            try
            {
                mLister.SearchCriteria = mLister.SearchCriteria ?? new Domain.Payout();
                mLister.Pagination.Take = mLister.Pagination.PageSize > 0 ? mLister.Pagination.PageSize : 0;
                mLister = _service.GetAllAsync(mLister).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            return PartialView("_List", mLister);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var brokers = _dropdownService.GetBrokerOptions(HttpContextHelper.SelectedBrokerId);
            var mPayout = new Domain.Payout
            {
                BrokerId = HttpContextHelper.SelectedBrokerId ?? (brokers.Count > 0 ? int.Parse(brokers[0].Value) : 0),
                ReceivedOn = DateTime.Today,
            };
            LoadDropdowns(mPayout);
            return PartialView("_Create", mPayout);
        }

        [HttpPost]
        public ActionResult Create(Domain.Payout mPayout, string statementMonthValue)
        {
            try
            {
                mPayout.StatementMonth = statementMonthValue.ToMonth();
                mPayout = Infrastructure.Helper.AuditHelper.ApplyAuditInfo(mPayout);
                var saved = _service.CreateAsync(mPayout).GetAwaiter().GetResult();
                var success = saved != null && saved.Id > 0;
                if (success) mPayout.Id = saved.Id;
                Notify(success, Enums.Operation.Save, Enums.MenuItem.Payout);
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                LoadDropdowns(mPayout);
            }
            return PartialView("_Create", mPayout);
        }

        [HttpGet]
        public ActionResult Get(int id)
        {
            var mPayout = new Domain.Payout();
            try
            {
                mPayout = _service.GetAsync(id).GetAwaiter().GetResult() ?? mPayout;
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                LoadDropdowns(mPayout);
            }
            return PartialView("_Create", mPayout);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                _service.DeleteAsync(id).GetAwaiter().GetResult();
                return JsonNotify(true, string.Format(Constant.SuccessMessage, "deleted", "payout"));
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        private void LoadDropdowns(Domain.Payout m)
        {
            ViewBag.Brokers = _dropdownService.GetBrokerOptions(m.BrokerId);
            var selected = m.StatementMonth.HasValue ? m.StatementMonth.Value.ToString("yyyy-MM-dd") : null;
            ViewBag.Months = _dropdownService.GetStatementMonthOptions(m.BrokerId > 0 ? m.BrokerId : (int?)null, selected);
        }
    }
}
