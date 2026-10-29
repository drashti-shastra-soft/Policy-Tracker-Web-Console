using PolicyTrackerWebClient.Infrastructure.IService;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class InsuranceCompanyService : ApiServiceBase, IInsuranceCompanyService
    {
        private readonly IHttpClientService _httpClientService;

        public InsuranceCompanyService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.InsuranceCompanyLister> GetAllAsync(Domain.InsuranceCompanyLister mLister)
        {
            return await _httpClientService.PostAsync<Domain.InsuranceCompanyLister, Domain.InsuranceCompanyLister>(
                url: $"{_baseUrl}/InsuranceCompany/GetAll",
                obj: mLister,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.InsuranceCompany> CreateAsync(Domain.InsuranceCompany mInsuranceCompany)
        {
            return await _httpClientService.PostAsync<Domain.InsuranceCompany, Domain.InsuranceCompany>(
                url: $"{_baseUrl}/InsuranceCompany",
                obj: mInsuranceCompany,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.InsuranceCompany> GetAsync(int id)
        {
            return await _httpClientService.GetAsync<Domain.InsuranceCompany>(
                url: $"{_baseUrl}/InsuranceCompany/{id}",
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClientService.DeleteAsync(
                url: $"{_baseUrl}/InsuranceCompany/{id}",
                token: Token
            ).ConfigureAwait(false);
        }
    }
}
