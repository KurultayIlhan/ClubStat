using ClubStat.Infrastructure.Infrastructure;

using ClubStatUI.Infrastructure;
using ClubStatUI.Platforms;

namespace ClubStatUI.Builder
{
    /*Verstoppen voor mensen die de dll laden om te kunnen misbruiken, zoals teamviewer(gebruikt om te hacken)*/
    public static class ClubStatsDependecyInjectionHelper
    {
        public static IServiceCollection RegisterWindows(this IServiceCollection services)
        {
            //  services.AddSingleton<MainPage>();
            //  services.AddSingleton<MainViewModel>();
            services.AddTransient<Pages.MainPage>();
            services.AddTransient<LoginViewModel>();

            services.AddTransient<Pages.DashboardCoach>();
            services.AddTransient<DashboardCoachViewModel>();

            services.AddTransient<Pages.DashboardPlayer>();
            services.AddTransient<DashboardPlayerViewModel>();

            services.AddTransient<Pages.TeamPlayer>();
            services.AddTransient<TeamPlayerViewModel>();

            services.AddTransient<Pages.DashboardCoach>();
            services.AddTransient<DashboardCoachViewModel>();

            services.AddTransient<Pages.ProfilePlayer>();
            services.AddTransient<ProfilePlayerViewModel>();
            
            services.AddTransient<Pages.StatsPlayer>();
            services.AddTransient<StatsPlayerViewModel>();

            services.AddTransient<Pages.AgendaPlayer>();
            services.AddTransient<AgendaPlayerViewModel>();


            services.AddTransient<Pages.FormationCoach>();
            services.AddTransient<FormationCoachViewModel>();

            services.AddTransient<ProfilePicturePlayerViewModel>();
            
            services.AddTransient<CurrentGameViewModel>();

            services.AddInfrastructure();
            services.AddSingleton<IOnlineDetector, OnlineDetector>();
            return services;
        }
        public static void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(Pages.MainPage), typeof(Pages.MainPage));
            Routing.RegisterRoute(nameof(Pages.DashboardCoach), typeof(Pages.DashboardCoach));
            Routing.RegisterRoute(nameof(Pages.DashboardPlayer), typeof(Pages.DashboardPlayer));
            Routing.RegisterRoute(nameof(Pages.TeamPlayer), typeof(Pages.TeamPlayer));
            Routing.RegisterRoute(nameof(Pages.StatsPlayer), typeof(Pages.StatsPlayer));
            Routing.RegisterRoute(nameof(Pages.FormationCoach), typeof(Pages.FormationCoach));
            Routing.RegisterRoute(nameof(Pages.CoachAbsences), typeof(Pages.CoachAbsences));

        }
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddPlatformDependencies();
        }
    }
}
