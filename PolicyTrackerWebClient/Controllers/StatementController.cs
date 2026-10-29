using PolicyTrackerWebClient.Filter;
using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    [Authentication]
    public class StatementController : BaseController
    {
        private const int MaxFileBytes = 15 * 1024 * 1024;

        private readonly IStatementService _service;
        private readonly IDropdownService _dropdownService;

        public StatementController(IStatementService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        public ActionResult Index(string matchStatus = null)
        {
            var mLister = new Domain.StatementLineLister();
            mLister.SearchCriteria.BrokerId = HttpContextHelper.SelectedBrokerId ?? 0;
            mLister.SearchCriteria.MatchStatus = matchStatus;
            mLister.Pagination.PageSize = 50;

            ViewBag.Brokers = _dropdownService.GetBrokerOptions(HttpContextHelper.SelectedBrokerId);
            return View(mLister);
        }

        public ActionResult List(Domain.StatementLineLister mLister)
        {
            try
            {
                mLister.SearchCriteria = mLister.SearchCriteria ?? new Domain.StatementLine();
                if (mLister.Pagination.PageSize == 0) mLister.Pagination.PageSize = 50;
                mLister.Pagination.Take = mLister.Pagination.PageSize > 0 ? mLister.Pagination.PageSize : 0;

                mLister = _service.GetAllAsync(mLister).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                var selectedMonth = mLister.SearchCriteria.StatementMonth > DateTime.MinValue ? mLister.SearchCriteria.StatementMonth.ToString("yyyy-MM-dd") : null;
                var brokerId = mLister.SearchCriteria.BrokerId > 0 ? mLister.SearchCriteria.BrokerId : (int?)null;
                ViewBag.Months = _dropdownService.GetStatementMonthOptions(brokerId, selectedMonth);
                ViewBag.Brokers = _dropdownService.GetBrokerOptions(brokerId);
                ViewBag.MatchStatuses = _dropdownService.GetEnumOptions<Enums.MatchStatus>(mLister.SearchCriteria.MatchStatus);
                ViewBag.Pagination = _dropdownService.GetPageSizeOptions(mLister.Pagination.PageSize);
            }

            return PartialView("_List", mLister);
        }

        /// <summary>Upload of the broker's commission Excel.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(int brokerId, string statementMonth, bool replaceExisting, HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                    return JsonNotify(false, "Please choose the broker's Excel (.xlsx) file.");

                if (file.ContentLength > MaxFileBytes)
                    return JsonNotify(false, "The file is too large (max 15 MB).");

                if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
                    return JsonNotify(false, "Only .xlsx files are supported. Open the file in Excel and use Save As → Excel Workbook (.xlsx).");

                byte[] bytes;
                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }

                var result = _service.ImportAsync(new Domain.StatementImport
                {
                    BrokerId = brokerId,
                    StatementMonth = statementMonth.ToMonth(),
                    FileName = Path.GetFileName(file.FileName),
                    FileBase64 = Convert.ToBase64String(bytes),
                    ReplaceExisting = replaceExisting,
                    CreatedBy = HttpContextHelper.LoginUser.Id,
                }).GetAwaiter().GetResult();

                HttpContextHelper.SelectedBrokerId = brokerId;

                return Json(new
                {
                    NotificationType = "success",
                    NotificationMsg = string.Format("Imported {0} rows ({1}) – {2} matched by policy no, {3} by name & date, {4} not matched.",
                        result.Imported, result.TotalValue.ToRupees(), result.MatchedByPolicyNo, result.MatchedByName, result.Unmatched),
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        [HttpGet]
        public ActionResult Link(int id)
        {
            var line = new Domain.StatementLine();
            try
            {
                line = _service.GetAsync(id).GetAwaiter().GetResult() ?? line;
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            return PartialView("_Link", line);
        }

        [HttpPost]
        public ActionResult Link(int statementLineId, string policyNo)
        {
            try
            {
                if (policyNo.IsNullOrEmpty()) return JsonNotify(false, "Enter the policy number to link.");
                _service.LinkAsync(new Domain.StatementLink { StatementLineId = statementLineId, PolicyNo = policyNo.Trim() }).GetAwaiter().GetResult();
                return JsonNotify(true, "Statement line linked to policy " + policyNo.Trim() + ".");
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        [HttpGet]
        public ActionResult Unlink(int id)
        {
            try
            {
                _service.LinkAsync(new Domain.StatementLink { StatementLineId = id, PolicyId = null }).GetAwaiter().GetResult();
                return JsonNotify(true, "Statement line unlinked.");
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        [HttpGet]
        public ActionResult CreatePolicy(int id)
        {
            try
            {
                var policy = _service.CreatePolicyAsync(id).GetAwaiter().GetResult();
                return JsonNotify(true, "Policy created for " + policy.CustomerName + ". Open Policies to add business type, payment and proof.");
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        [HttpGet]
        public ActionResult Rematch(int? brokerId)
        {
            try
            {
                brokerId = brokerId > 0 ? brokerId : HttpContextHelper.SelectedBrokerId;
                if (!brokerId.HasValue || brokerId <= 0) return JsonNotify(false, "Select a broker first.");

                var count = _service.RematchAsync(brokerId.Value).GetAwaiter().GetResult();
                return JsonNotify(true, count + " more statement line(s) matched.");
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                _service.DeleteAsync(id).GetAwaiter().GetResult();
                return JsonNotify(true, string.Format(Constant.SuccessMessage, "deleted", "statement line"));
            }
            catch (Exception ex)
            {
                return JsonNotify(false, ErrorText(ex));
            }
        }
    }
}
