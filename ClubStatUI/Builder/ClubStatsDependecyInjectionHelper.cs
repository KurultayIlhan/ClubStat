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

            services.AddSingleton<Pages.DashboardCoach>();
            services.AddSingleton<DashboardCoachViewModel>();

            services.AddSingleton<Pages.DashboardPlayer>();
            services.AddSingleton<DashboardPlayerViewModel>();

            services.AddSingleton<Pages.TeamPlayer>();
            services.AddSingleton<TeamPlayerViewModel>();

            services.AddSingleton<Pages.DashboardCoach>();
            services.AddSingleton<DashboardCoachViewModel>();
            services.AddTransient<ProfilePicturePlayerViewModel>();
            services.AddTransient<TeamPlayerViewModel>();
            services.AddTransient<CurrentGameViewModel>();

            services.AddInfrastructure();
            return services;
        }
        public static void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(Pages.MainPage), typeof(Pages.MainPage));
            Routing.RegisterRoute(nameof(Pages.DashboardCoach), typeof(Pages.DashboardCoach));
            Routing.RegisterRoute(nameof(Pages.DashboardPlayer), typeof(Pages.DashboardPlayer));
            Routing.RegisterRoute(nameof(Pages.TeamPlayer), typeof(Pages.TeamPlayer));
            Routing.RegisterRoute(nameof(Pages.StatsPlayer), typeof(Pages.StatsPlayer));

        }
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddPlatformDependencies();
        }
    }
}
