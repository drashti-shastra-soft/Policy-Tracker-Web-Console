using PolicyTrackerWebClient.Filter;
using System.Web.Mvc;

namespace PolicyTrackerWebClient
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new SessionExpiredFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
