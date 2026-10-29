using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Infrastructure.IService
{
    public interface IAuthenticationService
    {
        Task<Domain.User> AuthenticateAsync(Domain.Credential mCredential);
    }
}
