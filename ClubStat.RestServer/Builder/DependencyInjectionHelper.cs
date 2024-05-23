using ClubStat.Infrastructure.Settings;
using ClubStat.RestServer.Infrastructure;

using Microsoft.Extensions.Caching.Memory;

namespace ClubStat.RestServer.Builder
{
    public static class DependencyInjectionHelper
    {

        static object _locker = new object();
        public static T AddWebsiteInfrastructure<T>(this T service) where T : IServiceCollection
        {
            service.AddTransient<DbHelper>();
            service.AddLogging();
            service.AddTransient<ApiSettings>(service => GetApiKey(service));
            //Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False
            return service;
        }

        /// <summary>
        /// Gets the API key using a thread-safe helper mathod.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>ApiSettings.</returns>
        private static ApiSettings GetApiKey(IServiceProvider service)
        {
            //if cahsed return

            var memory = service.GetRequiredService<IMemoryCache>();
            if (memory.TryGetValue<ApiSettings>(nameof(ApiSettings), out var apiSettings) && apiSettings is not null)
            {
                return apiSettings;
            }
            lock (_locker)
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                apiSettings = new ApiSettings() { ApiKey = string.Empty, Url = "https://ilhankurultay-001-site1.btempurl.com/" };
                configuration.Bind("Api", apiSettings);

                var options = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1), Priority = CacheItemPriority.Low};
                options.ExpirationTokens.Add(configuration.GetReloadToken());//reload when configuration gets changed on disk

                memory.Set<ApiSettings>(nameof(ApiSettings), apiSettings,options );

                return apiSettings;
            }


        }
    }
}
