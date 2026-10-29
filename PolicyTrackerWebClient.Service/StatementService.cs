using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class StatementService : ApiServiceBase, IStatementService
    {
        private readonly IHttpClientService _httpClientService;

        public StatementService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.StatementImportResult> ImportAsync(Domain.StatementImport mImport)
        {
            return await _httpClientService.PostAsync<Domain.StatementImport, Domain.StatementImportResult>($"{_baseUrl}/Statement/Import", mImport, Token).ConfigureAwait(false);
        }

        public async Task<Domain.StatementLineLister> GetAllAsync(Domain.StatementLineLister mLister)
        {
            return await _httpClientService.PostAsync<Domain.StatementLineLister, Domain.StatementLineLister>($"{_baseUrl}/Statement/GetAll", mLister, Token).ConfigureAwait(false);
        }

        public async Task<Domain.StatementLine> GetAsync(int id)
        {
            return await _httpClientService.GetAsync<Domain.StatementLine>($"{_baseUrl}/Statement/{id}", Token).ConfigureAwait(false);
        }

        public async Task<Domain.StatementLine> LinkAsync(Domain.StatementLink mLink)
        {
            return await _httpClientService.PostAsync<Domain.StatementLink, Domain.StatementLine>($"{_baseUrl}/Statement/Link", mLink, Token).ConfigureAwait(false);
        }

        public async Task<Domain.Policy> CreatePolicyAsync(int lineId)
        {
            return await _httpClientService.PostAsync<object, Domain.Policy>($"{_baseUrl}/Statement/{lineId}/CreatePolicy", new { }, Token).ConfigureAwait(false);
        }

        public async Task<int> RematchAsync(int brokerId)
        {
            return await _httpClientService.PostAsync<object, int>($"{_baseUrl}/Statement/Rematch/{brokerId}", new { }, Token).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClientService.DeleteAsync($"{_baseUrl}/Statement/{id}", Token).ConfigureAwait(false);
        }

        public async Task<List<DateTime>> GetMonthsAsync(int? brokerId)
        {
            return await _httpClientService.GetAsync<List<DateTime>>($"{_baseUrl}/Statement/Months?brokerId={brokerId}", Token).ConfigureAwait(false);
        }
    }
}
