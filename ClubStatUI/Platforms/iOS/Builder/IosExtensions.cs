using ClubStat.Infrastructure;

using ClubStatUI.Platforms.iOS;
namespace ClubStatUI.Platforms
{
    static class AndroidExtensions
    {
        public static T AddPlatformDependencies<T>(this T service) where T : IServiceCollection
        {
            service.AddSingleton<IPlayerDynamics, IosPlayerDynamics>();
            service.AddTransient<IMessageDialog, IosMessage>();
            return service;
        }
    }
}
