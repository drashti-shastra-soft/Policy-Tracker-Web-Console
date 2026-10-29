using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Infrastructure.IService
{
    public interface IPayoutService
    {
        Task<Domain.PayoutLister> GetAllAsync(Domain.PayoutLister mLister);

        Task<Domain.Payout> CreateAsync(Domain.Payout mPayout);

        Task<Domain.Payout> GetAsync(int id);

        Task DeleteAsync(int id);
    }
}
