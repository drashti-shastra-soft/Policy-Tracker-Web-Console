using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Infrastructure.IService
{
    public interface IStatementService
    {
        Task<Domain.StatementImportResult> ImportAsync(Domain.StatementImport mImport);

        Task<Domain.StatementLineLister> GetAllAsync(Domain.StatementLineLister mLister);

        Task<Domain.StatementLine> GetAsync(int id);

        Task<Domain.StatementLine> LinkAsync(Domain.StatementLink mLink);

        Task<Domain.Policy> CreatePolicyAsync(int lineId);

        Task<int> RematchAsync(int brokerId);

        Task DeleteAsync(int id);

        Task<List<DateTime>> GetMonthsAsync(int? brokerId);
    }
}
