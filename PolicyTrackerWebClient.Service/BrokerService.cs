using PolicyTrackerWebClient.Infrastructure.IService;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class BrokerService : ApiServiceBase, IBrokerService
    {
        private readonly IHttpClientService _httpClientService;

        public BrokerService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.BrokerLister> GetAllAsync(Domain.BrokerLister mLister)
        {
            return await _httpClientService.PostAsync<Domain.BrokerLister, Domain.BrokerLister>(
                url: $"{_baseUrl}/Broker/GetAll",
                obj: mLister,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.Broker> CreateAsync(Domain.Broker mBroker)
        {
            return await _httpClientService.PostAsync<Domain.Broker, Domain.Broker>(
                url: $"{_baseUrl}/Broker",
                obj: mBroker,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.Broker> GetAsync(int id)
        {
            return await _httpClientService.GetAsync<Domain.Broker>(
                url: $"{_baseUrl}/Broker/{id}",
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClientService.DeleteAsync(
                url: $"{_baseUrl}/Broker/{id}",
                token: Token
            ).ConfigureAwait(false);
        }
    }
}
