using PolicyTrackerWebClient.Infrastructure.IService;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class CategoryService : ApiServiceBase, ICategoryService
    {
        private readonly IHttpClientService _httpClientService;

        public CategoryService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.CategoryLister> GetAllAsync(Domain.CategoryLister mLister)
        {
            return await _httpClientService.PostAsync<Domain.CategoryLister, Domain.CategoryLister>(
                url: $"{_baseUrl}/Category/GetAll",
                obj: mLister,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.Category> CreateAsync(Domain.Category mCategory)
        {
            return await _httpClientService.PostAsync<Domain.Category, Domain.Category>(
                url: $"{_baseUrl}/Category",
                obj: mCategory,
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task<Domain.Category> GetAsync(int id)
        {
            return await _httpClientService.GetAsync<Domain.Category>(
                url: $"{_baseUrl}/Category/{id}",
                token: Token
            ).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClientService.DeleteAsync(
                url: $"{_baseUrl}/Category/{id}",
                token: Token
            ).ConfigureAwait(false);
        }
    }
}
