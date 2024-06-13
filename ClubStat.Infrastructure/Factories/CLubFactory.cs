// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : Sat 01-Jun-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Sat 01-Jun-2024
// ***********************************************************************
// <copyright file="CLubFactory.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.Caching.Memory;

namespace ClubStat.Infrastructure.Factories
{
    public interface IClubFactory
    {
        Task<List<Player>> GetTeamPlayers(int clubId, int playersLeague, char playersLeagueLevel);
    }
    internal class ClubFactory : ApiBasedFactory, IClubFactory
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IProfilePictureFactory _pictureFactory;

        public ClubFactory(IConfiguration configuration, IHttpClientFactory clientFactory, IMemoryCache memoryCache, IProfilePictureFactory pictureFactory)
            : base(configuration, clientFactory)
        {
            _memoryCache = memoryCache;
            _pictureFactory = pictureFactory;
        }

        public async Task<List<Player>> GetTeamPlayers(int clubId, int playersLeague, char playersLeagueLevel)
        {
            var url = $"api/club/GetTeam/{clubId}/{playersLeague}/{playersLeagueLevel}";
            if (_memoryCache.TryGetValue<List<Player>>(url, out var cashed) && cashed is not null && cashed.Count > 0)
            {
                return cashed;
            }
            var team = await base.GetAsync<List<Player>>(url, TeamOfPlayerJsonContext.Default.ListPlayer).ConfigureAwait(false);
            if (team is not null && team.Count > 0)
            {
                var tasks = team.Select(async player => {
                    try
                    {
                        byte[]? bytes = await _pictureFactory.GetProfilePictureForUserAsync(player.UserId).ConfigureAwait(false);
                        if (bytes is not null && bytes.Length > 0)
                        {
                            player.ProfileImageBytes = bytes;
                        }
                    }
                    catch (Exception ex)
                    {
                        Walter.Inverse.GetLogger("ClubFactory.GetTeamPlayers")?.LogException(ex);
                    }
                });

                await Task.WhenAll(tasks);
                _memoryCache.Set(url, team, absoluteExpiration: new DateTimeOffset(DateTime.Now.AddMinutes(10)));
            }
            else
            {
                team = new();
            }
            return team;
        }


    }
}
