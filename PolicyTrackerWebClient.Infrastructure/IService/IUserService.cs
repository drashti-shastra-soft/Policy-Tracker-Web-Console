using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Infrastructure.IService
{
    public interface IUserService
    {
        Task<Domain.UserLister> GetAllAsync(Domain.UserLister mLister);

        Task<Domain.User> CreateAsync(Domain.User mUser);

        Task<Domain.User> GetAsync(int id);

        Task DeleteAsync(int id);

        Task ChangePasswordAsync(Domain.ChangePassword mChangePassword);
    }
}
