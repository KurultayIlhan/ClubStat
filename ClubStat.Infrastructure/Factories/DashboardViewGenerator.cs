// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Sun 12-May-2024
// ***********************************************************************
// <copyright file="DashboardViewGenerator.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStat.Infrastructure.Factories
{
    public interface IDashboardViewGenerator
    {
        Task<DashboardPlayerView> GetViewForAsync(IAuthenticatedUser loggedInUser);

    }
    internal class DashboardViewGenerator : ApiBasedFactory, IDashboardViewGenerator
    {
        public DashboardViewGenerator(IConfiguration configuration, IHttpClientFactory clientFactory)
            : base(configuration, clientFactory)
        {

        }

        public async Task<DashboardPlayerView> GetViewForAsync(IAuthenticatedUser loggedInUser)
        {
            var api = loggedInUser.DashboardUrlFor();
            var result = await base.GetAsync<DashboardPlayerView>(api)
                            .ConfigureAwait(false);
            return result ?? throw new ApplicationException("Can't parse result");
        }
    }
}
