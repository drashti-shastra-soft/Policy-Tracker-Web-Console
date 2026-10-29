using PolicyTrackerWebClient.Infrastructure.IService;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    public class AuthenticationService : ApiServiceBase, IAuthenticationService
    {
        private readonly IHttpClientService _httpClientService;

        public AuthenticationService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public async Task<Domain.User> AuthenticateAsync(Domain.Credential mCredential)
        {
            return await _httpClientService.PostAsync<Domain.Credential, Domain.User>(
                url: $"{_baseUrl}/Authentication",
                obj: mCredential
            ).ConfigureAwait(false);
        }
    }
}
