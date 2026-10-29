using PolicyTrackerWebClient.Infrastructure.IService;
using PolicyTrackerWebClient.Service;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace PolicyTrackerWebClient
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            container.RegisterType<IAuthenticationService, AuthenticationService>();
            container.RegisterType<IBrokerService, BrokerService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IDropdownService, DropdownService>();
            container.RegisterType<IHttpClientService, HttpClientService>();
            container.RegisterType<IInsuranceCompanyService, InsuranceCompanyService>();
            container.RegisterType<IPayoutService, PayoutService>();
            container.RegisterType<IPointsGridService, PointsGridService>();
            container.RegisterType<IPolicyService, PolicyService>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<IStatementService, StatementService>();
            container.RegisterType<IUserService, UserService>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
