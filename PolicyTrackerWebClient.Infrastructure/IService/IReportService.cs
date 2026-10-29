using System;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Infrastructure.IService
{
    public interface IReportService
    {
        Task<Domain.DashboardSummary> GetDashboardAsync(DateTime month, int? brokerId);

        Task<Domain.ReconciliationReport> GetReconciliationAsync(int? brokerId);
    }
}
