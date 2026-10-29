using PolicyTrackerWebClient.Infrastructure.IService;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class PointsGridService : ApiServiceBase, IPointsGridService
    {
        private readonly IHttpClientService _httpClientService;

        public PointsGridService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.PointsGridLister> GetAllAsync(Domain.PointsGridLister mLister)
        {
            return await _httpClientService.PostAsync<Domain.PointsGridLister, Domain.PointsGridLister>(
                url: $"{_baseUrl}/PointsGrid/GetAll",
                obj: mLister,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.PointsGrid> CreateAsync(Domain.PointsGrid mPointsGrid)
        {
            return await _httpClientService.PostAsync<Domain.PointsGrid, Domain.PointsGrid>(
                url: $"{_baseUrl}/PointsGrid",
                obj: mPointsGrid,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.PointsGrid> GetAsync(int id)
        {
            return await _httpClientService.GetAsync<Domain.PointsGrid>(
                url: $"{_baseUrl}/PointsGrid/{id}",
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClientService.DeleteAsync(
                url: $"{_baseUrl}/PointsGrid/{id}",
                token: Token
            ).ConfigureAwait(false);
        }
    }
}
