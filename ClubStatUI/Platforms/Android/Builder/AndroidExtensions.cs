using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClubStat.Infrastructure;
//using ClubStat.Infrastructure.Infrastructure;
using ClubStatUI.Platforms.Android;
namespace ClubStatUI.Platforms
{
   static class AndroidExtensions
    {
        public static T AddPlatformDependencies<T>(this T service)where T:IServiceCollection 
        {
            service.AddSingleton<IPlayerDynamics, AndroidPlayerDynamics>();
            service.AddTransient<IMessageDialog, AndroidMessage>();
            return service;
        }
    }
}
