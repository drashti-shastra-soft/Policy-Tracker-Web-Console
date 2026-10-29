using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class ReportService : ApiServiceBase, IReportService
    {
        private readonly IHttpClientService _httpClientService;

        public ReportService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.DashboardSummary> GetDashboardAsync(DateTime month, int? brokerId)
        {
            var url = string.Format(CultureInfo.InvariantCulture, "{0}/Report/Dashboard?month={1:yyyy-MM-dd}&brokerId={2}", _baseUrl, month, brokerId);
            return await _httpClientService.GetAsync<Domain.DashboardSummary>(url, Token).ConfigureAwait(false);
        }

        public async Task<Domain.ReconciliationReport> GetReconciliationAsync(int? brokerId)
        {
            return await _httpClientService.GetAsync<Domain.ReconciliationReport>($"{_baseUrl}/Report/Reconciliation?brokerId={brokerId}", Token).ConfigureAwait(false);
        }
    }
}
