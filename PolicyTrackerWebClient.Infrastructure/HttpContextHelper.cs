using System.Web;

namespace PolicyTrackerWebClient.Infrastructure
{
    public class HttpContextHelper
    {
        private const string LoginUserKey = "LoginUser";
        private const string BrokerKey = "SelectedBrokerId";

        public static Domain.User LoginUser
        {
            get => HttpContext.Current?.Session?[LoginUserKey] as Domain.User;
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[LoginUserKey] = value;
            }
        }

        public static bool IsAdmin
        {
            get { return LoginUser != null && LoginUser.RoleId == EnumsHelper.GetEnumIndex(Enums.Role.Admin); }
        }

        /// <summary>Broker chosen on the dashboard, remembered for the session.</summary>
        public static int? SelectedBrokerId
        {
            get => HttpContext.Current?.Session?[BrokerKey] as int?;
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Session[BrokerKey] = value;
            }
        }
    }
}
