using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Infrastructure.IService
{
    public interface IPointsGridService
    {
        Task<Domain.PointsGridLister> GetAllAsync(Domain.PointsGridLister mLister);

        Task<Domain.PointsGrid> CreateAsync(Domain.PointsGrid mPointsGrid);

        Task<Domain.PointsGrid> GetAsync(int id);

        Task DeleteAsync(int id);
    }
}
