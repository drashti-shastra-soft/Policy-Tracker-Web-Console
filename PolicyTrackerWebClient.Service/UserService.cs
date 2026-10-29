using PolicyTrackerWebClient.Infrastructure.IService;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class UserService : ApiServiceBase, IUserService
    {
        private readonly IHttpClientService _httpClientService;

        public UserService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.UserLister> GetAllAsync(Domain.UserLister mLister)
        {
            return await _httpClientService.PostAsync<Domain.UserLister, Domain.UserLister>($"{_baseUrl}/User/GetAll", mLister, Token).ConfigureAwait(false);
        }

        public async Task<Domain.User> CreateAsync(Domain.User mUser)
        {
            return await _httpClientService.PostAsync<Domain.User, Domain.User>($"{_baseUrl}/User", mUser, Token).ConfigureAwait(false);
        }

        public async Task<Domain.User> GetAsync(int id)
        {
            return await _httpClientService.GetAsync<Domain.User>($"{_baseUrl}/User/{id}", Token).ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id)
        {
            await _httpClientService.DeleteAsync($"{_baseUrl}/User/{id}", Token).ConfigureAwait(false);
        }

        public async Task ChangePasswordAsync(Domain.ChangePassword mChangePassword)
        {
            await _httpClientService.PostAsync<Domain.ChangePassword, object>($"{_baseUrl}/User/ChangePassword", mChangePassword, Token).ConfigureAwait(false);
        }
    }
}
