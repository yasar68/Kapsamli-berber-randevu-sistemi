using Hangfire.Dashboard;

namespace BerberApp.API.Helpers
{
    public class HangfireDashboardAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // ⚠️ Geliştirme/test ortamı için herkese açık
            return true;
        }
    }
}
