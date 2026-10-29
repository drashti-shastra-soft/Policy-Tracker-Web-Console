using System.Configuration;

namespace PolicyTrackerWebClient.Service
{
    public abstract class ApiServiceBase
    {
        protected readonly string _baseUrl;

        protected ApiServiceBase()
        {
            _baseUrl = (ConfigurationManager.AppSettings.Get("APIBaseUrl") ?? string.Empty).TrimEnd('/');
        }

        /// <summary>Token of the logged-in user (read on every call, never cached).</summary>
        protected static string Token
        {
            get { return Infrastructure.HttpContextHelper.LoginUser?.Token; }
        }
    }
}
