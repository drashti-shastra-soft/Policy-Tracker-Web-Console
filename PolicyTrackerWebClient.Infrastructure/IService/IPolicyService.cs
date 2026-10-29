using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Infrastructure.IService
{
    public interface IPolicyService
    {
        Task<Domain.PolicyLister> GetAllAsync(Domain.PolicyLister mLister);

        Task<Domain.Policy> CreateAsync(Domain.Policy mPolicy);

        Task<Domain.Policy> GetAsync(int id);

        Task DeleteAsync(int id);

        Task<Domain.RenewalFollowUp> FollowUpAsync(Domain.RenewalFollowUp mFollowUp);

        Task<List<Domain.Policy>> GetDueListAsync(DateTime month, int? brokerId, bool includeClosed);
    }
}
