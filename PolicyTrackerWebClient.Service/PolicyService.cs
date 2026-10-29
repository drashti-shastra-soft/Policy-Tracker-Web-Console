using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class PolicyService : ApiServiceBase, IPolicyService
    {
        private readonly IHttpClientService _httpClientService;

        public PolicyService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.PolicyLister> GetAllAsync(Domain.PolicyLister mLister)
        {
            return await _httpClientService.PostAsync<Domain.PolicyLister, Domain.PolicyLister>($"{_baseUrl}/Policy/GetAll", mLister, Token).ConfigureAwait(false);
        }

        public async Task<Domain.Policy> CreateAsync(Domain.Policy mPolicy)
        {
            return await _httpClientService.PostAsync<Domain.Policy, Domain.Policy>($"{_baseUrl}/Policy", mPolicy, Token).ConfigureAwait(false);
        }

        public async Task<Domain.Policy> GetAsync(int id)
        {
            return await _httpClientService.GetAsync<Domain.Policy>($"{_baseUrl}/Policy/{id}", Token).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClientService.DeleteAsync($"{_baseUrl}/Policy/{id}", Token).ConfigureAwait(false);
        }

        public async Task<Domain.RenewalFollowUp> FollowUpAsync(Domain.RenewalFollowUp mFollowUp)
        {
            return await _httpClientService.PostAsync<Domain.RenewalFollowUp, Domain.RenewalFollowUp>($"{_baseUrl}/Policy/FollowUp", mFollowUp, Token).ConfigureAwait(false);
        }

        public async Task<List<Domain.Policy>> GetDueListAsync(DateTime month, int? brokerId, bool includeClosed)
        {
            var url = string.Format(CultureInfo.InvariantCulture, "{0}/Policy/DueList?month={1:yyyy-MM-dd}&brokerId={2}&includeClosed={3}",
                                    _baseUrl, month, brokerId, includeClosed.ToString().ToLowerInvariant());
            return await _httpClientService.GetAsync<List<Domain.Policy>>(url, Token).ConfigureAwait(false);
        }
    }
}
