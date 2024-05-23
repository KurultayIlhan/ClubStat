// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
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
        public static bool InjectFake = false;
        public static T RegisterClubStats<T>(this T services, IConfiguration configuration) where T : IServiceCollection
        {
            services.AddSingleton<IConfiguration>(configuration)
                    .AddSingleton<IDashboardViewGenerator, DashboardViewGenerator>();
            if (InjectFake)
            {
                //inject fakes for development and automation testing 
                //without working on live data
            }
            else 
            {
                //live data
                services.InjectFactories();
            }

            return services;
        }

        //inject the live versions
        static T InjectFactories<T>(this T services) where T : IServiceCollection
        {
            services.AddSingleton<ILoginFactory, LoginFactory>();
            services.AddTransient<LoggedInUser>(services => services.GetRequiredService<ILoginFactory>().CurrentUser!);
            return services;
        }
    }
}
