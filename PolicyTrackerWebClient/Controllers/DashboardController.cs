using PolicyTrackerWebClient.Filter;
using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Controllers
{
    [Authentication]
    public class DashboardController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly IDropdownService _dropdownService;

        public DashboardController(IReportService reportService, IDropdownService dropdownService)
        {
            _reportService = reportService;
            _dropdownService = dropdownService;
        }

        /// <param name="month">yyyy-MM from the month picker.</param>
        public ActionResult Index(string month = null, int? brokerId = null)
        {
            var selectedMonth = month.ToMonth() ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            if (brokerId.HasValue) HttpContextHelper.SelectedBrokerId = brokerId > 0 ? brokerId : null;

            var model = new Domain.DashboardSummary { Month = selectedMonth };
            try
            {
                model = _reportService.GetDashboardAsync(selectedMonth, HttpContextHelper.SelectedBrokerId).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
            finally
            {
                ViewBag.Brokers = _dropdownService.GetBrokerOptions(HttpContextHelper.SelectedBrokerId);
                ViewBag.Month = selectedMonth.ToString("yyyy-MM");
            }

            return View(model);
        }
    }
}
