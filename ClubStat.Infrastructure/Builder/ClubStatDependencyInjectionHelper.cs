// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Tue 14-May-2024
// ***********************************************************************
// <copyright file="ClubStatDependencyInjectionHelper.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace ClubStat.Infrastructure.Builder
{
    public static class ClubStatDependencyInjectionHelper
    {
        public static T RegisterClubStats<T>(this T services, IConfiguration configuration) where T : IServiceCollection
        {
            services.AddSingleton<IConfiguration>(configuration)
                    .AddSingleton<IDashboardViewGenerator, DashboardViewGenerator>()
                    .AddHttpClient()
                    .AddMemoryCache();
                    
                //live data
                services.InjectFactories();

            return services;
        }

        //inject the live versions
        static T InjectFactories<T>(this T services) where T : IServiceCollection
        {
            services.AddSingleton<IPlayerRecorder, PlayerRecorder>();
            services.AddSingleton<ILoginFactory, LoginFactory>();
            services.AddSingleton<IMatchFactory, MatchFactory>();
            services.AddSingleton<IClubFactory, ClubFactory>();
            services.AddTransient<IPlayerMentainance, PlayerMentainance>();
            services.AddTransient<IProfilePictureFactory,ProfilePictureFactory>();
            services.AddTransient<LoggedInUser>(services => services.GetRequiredService<ILoginFactory>().CurrentUser!);
            return services;
        }
    }
}
