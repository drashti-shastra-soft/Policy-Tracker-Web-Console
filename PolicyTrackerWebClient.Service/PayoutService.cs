using PolicyTrackerWebClient.Infrastructure.IService;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class PayoutService : ApiServiceBase, IPayoutService
    {
        private readonly IHttpClientService _httpClientService;

        public PayoutService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.PayoutLister> GetAllAsync(Domain.PayoutLister mLister)
        {
            return await _httpClientService.PostAsync<Domain.PayoutLister, Domain.PayoutLister>(
                url: $"{_baseUrl}/Payout/GetAll",
                obj: mLister,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.Payout> CreateAsync(Domain.Payout mPayout)
        {
            return await _httpClientService.PostAsync<Domain.Payout, Domain.Payout>(
                url: $"{_baseUrl}/Payout",
                obj: mPayout,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.Payout> GetAsync(int id)
        {
            return await _httpClientService.GetAsync<Domain.Payout>(
                url: $"{_baseUrl}/Payout/{id}",
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClientService.DeleteAsync(
                url: $"{_baseUrl}/Payout/{id}",
                token: Token
            ).ConfigureAwait(false);
        }
    }
}
