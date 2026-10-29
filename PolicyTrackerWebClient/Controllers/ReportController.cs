using PolicyTrackerWebClient.Filter;
using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    [Authentication]
    public class ReportController : BaseController
    {
        private readonly IReportService _service;
        private readonly IDropdownService _dropdownService;

        public ReportController(IReportService service, IDropdownService dropdownService)
        {
            _service = service;
            _dropdownService = dropdownService;
        }

        /// <summary>Statement vs bank reconciliation – the list to discuss with the broker.</summary>
        public ActionResult Index(int? brokerId = null)
        {
            if (brokerId.HasValue) HttpContextHelper.SelectedBrokerId = brokerId > 0 ? brokerId : null;

            var model = new Domain.ReconciliationReport();
            try
            {
                model = _service.GetReconciliationAsync(HttpContextHelper.SelectedBrokerId).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                ViewBag.Brokers = _dropdownService.GetBrokerOptions(HttpContextHelper.SelectedBrokerId);
            }
            return View(model);
        }
    }
}
