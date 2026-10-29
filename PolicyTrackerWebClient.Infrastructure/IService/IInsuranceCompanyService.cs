using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Infrastructure.IService
{
    public interface IInsuranceCompanyService
    {
        Task<Domain.InsuranceCompanyLister> GetAllAsync(Domain.InsuranceCompanyLister mLister);

        Task<Domain.InsuranceCompany> CreateAsync(Domain.InsuranceCompany mInsuranceCompany);

        Task<Domain.InsuranceCompany> GetAsync(int id);

        Task DeleteAsync(int id);
    }
}
