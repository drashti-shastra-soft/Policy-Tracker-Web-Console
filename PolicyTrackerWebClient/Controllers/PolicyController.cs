using PolicyTrackerWebClient.Filter;
using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    [Authentication]
    public class PolicyController : BaseController
    {
        private readonly IPolicyService _service;
        private readonly IDropdownService _dropdownService;

        public PolicyController(IPolicyService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        public ActionResult Index(string statementStatus = null, string renewalStatus = null)
        {
            var mLister = new Domain.PolicyLister();
            mLister.SearchCriteria.BrokerId = HttpContextHelper.SelectedBrokerId ?? 0;
            mLister.SearchCriteria.StatementStatus = statementStatus;
            mLister.SearchCriteria.RenewalStatus = renewalStatus;
            mLister.Pagination.PageSize = 20;
            return View(mLister);
        }

        public ActionResult List(Domain.PolicyLister mLister)
        {
            try
            {
                mLister.SearchCriteria = mLister.SearchCriteria ?? new Domain.Policy();
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
                LoadDropdowns(mLister.SearchCriteria);
                ViewBag.Pagination = _dropdownService.GetPageSizeOptions(mLister.Pagination.PageSize);
            }

            return PartialView("_List", mLister);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var brokers = _dropdownService.GetBrokerOptions(HttpContextHelper.SelectedBrokerId);
            var mPolicy = new Domain.Policy
            {
                BrokerId = HttpContextHelper.SelectedBrokerId ?? (brokers.Count > 0 ? int.Parse(brokers[0].Value) : 0),
                BusinessType = EnumsHelper.GetEnumString(Enums.BusinessType.Fresh),
                RenewalStatus = EnumsHelper.GetEnumString(Enums.RenewalStatus.Due),
                StartDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddYears(1).AddDays(-1),
                SubmittedToBrokerOn = DateTime.Today,
            };
            LoadDropdowns(mPolicy);
            return PartialView("_Create", mPolicy);
        }

        [HttpPost]
        public ActionResult Create(Domain.Policy mPolicy)
        {
            try
            {
                mPolicy = Infrastructure.Helper.AuditHelper.ApplyAuditInfo(mPolicy);
                var saved = _service.CreateAsync(mPolicy).GetAwaiter().GetResult();
                var success = saved != null && saved.Id > 0;
                if (success) mPolicy.Id = saved.Id;
                Notify(success, Enums.Operation.Save, Enums.MenuItem.Policy);
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                LoadDropdowns(mPolicy);
            }
            return PartialView("_Create", mPolicy);
        }

        [HttpGet]
        public ActionResult Get(int id)
        {
            var mPolicy = new Domain.Policy();
            try
            {
                mPolicy = _service.GetAsync(id).GetAwaiter().GetResult() ?? new Domain.Policy();
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                LoadDropdowns(mPolicy);
            }
            return PartialView("_Create", mPolicy);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                _service.DeleteAsync(id).GetAwaiter().GetResult();
                return JsonNotify(true, string.Format(Constant.SuccessMessage, "deleted", "policy"));
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        // ---------------- Renewal follow-up (options instead of handwriting) ----------------

        [HttpGet]
        public ActionResult FollowUp(int id)
        {
            var mFollowUp = new Domain.RenewalFollowUp { PolicyId = id };
            try
            {
                var policy = _service.GetAsync(id).GetAwaiter().GetResult();
                ViewBag.Policy = policy;

                if (policy != null)
                {
                    var newStart = policy.ExpiryDate.HasValue ? policy.ExpiryDate.Value.AddDays(1) : DateTime.Today;
                    mFollowUp.RenewalStatus = EnumsHelper.Is(policy.RenewalStatus, Enums.RenewalStatus.Due) || policy.RenewalStatus.IsNullOrEmpty()
                        ? EnumsHelper.GetEnumString(Enums.RenewalStatus.Quote_Sent)
                        : policy.RenewalStatus;
                    mFollowUp.LostReason = policy.LostReason;
                    mFollowUp.QuoteSentOn = policy.QuoteSentOn ?? DateTime.Today;
                    mFollowUp.QuoteDetails = policy.QuoteDetails;
                    mFollowUp.RenewedOn = policy.RenewedOn ?? DateTime.Today;
                    mFollowUp.NewStartDate = newStart;
                    mFollowUp.NewExpiryDate = newStart.AddYears(1).AddDays(-1);
                    mFollowUp.InsuranceCompanyId = policy.InsuranceCompanyId;
                    mFollowUp.SumInsured = policy.SumInsured;
                    mFollowUp.NetPremium = policy.IndicativePremium;
                    mFollowUp.PaymentDate = DateTime.Today;
                    mFollowUp.SubmittedToBrokerOn = DateTime.Today;
                    mFollowUp.Remarks = policy.Remarks;
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                LoadFollowUpDropdowns(mFollowUp);
            }
            return PartialView("_FollowUp", mFollowUp);
        }

        [HttpPost]
        public ActionResult FollowUp(Domain.RenewalFollowUp mFollowUp)
        {
            try
            {
                mFollowUp.LastModifiedBy = HttpContextHelper.LoginUser.Id;
                var result = _service.FollowUpAsync(mFollowUp).GetAwaiter().GetResult();
                Notify(result != null, Enums.Operation.Save, Enums.MenuItem.Renewal);
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                try { ViewBag.Policy = _service.GetAsync(mFollowUp.PolicyId).GetAwaiter().GetResult(); } catch (Exception) { ViewBag.Policy = null; }
                LoadFollowUpDropdowns(mFollowUp);
            }
            return PartialView("_FollowUp", mFollowUp);
        }

        private void LoadDropdowns(Domain.Policy m)
        {
            m = m ?? new Domain.Policy();
            ViewBag.Brokers = _dropdownService.GetBrokerOptions(m.BrokerId);
            ViewBag.Categories = _dropdownService.GetCategoryOptions(m.CategoryId);
            ViewBag.Companies = _dropdownService.GetInsuranceCompanyOptions(m.InsuranceCompanyId);
            ViewBag.BusinessTypes = _dropdownService.GetEnumOptions<Enums.BusinessType>(m.BusinessType);
            ViewBag.RenewalStatuses = _dropdownService.GetEnumOptions<Enums.RenewalStatus>(m.RenewalStatus);
            ViewBag.StatementStatuses = _dropdownService.GetEnumOptions<Enums.StatementStatus>(m.StatementStatus);
            ViewBag.PaymentModes = _dropdownService.GetEnumOptions<Enums.PaymentMode>(m.PaymentMode);
            ViewBag.LostReasons = _dropdownService.GetEnumOptions<Enums.LostReason>(m.LostReason);
        }

        private void LoadFollowUpDropdowns(Domain.RenewalFollowUp m)
        {
            ViewBag.RenewalStatuses = _dropdownService.GetEnumOptions<Enums.RenewalStatus>(m.RenewalStatus)
                                                      .Where(x => x.Value != EnumsHelper.GetEnumString(Enums.RenewalStatus.Due)).ToList();
            ViewBag.LostReasons = _dropdownService.GetEnumOptions<Enums.LostReason>(m.LostReason);
            ViewBag.PaymentModes = _dropdownService.GetEnumOptions<Enums.PaymentMode>(m.PaymentMode);
            ViewBag.Companies = _dropdownService.GetInsuranceCompanyOptions(m.InsuranceCompanyId);
        }
    }
}
