// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : Mon 27-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Mon 27-May-2024
// ***********************************************************************
// <copyright file="PlayerMentainance.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary>
// Save Motivation in memory when the user is a coeach to make 
// web requests less frequent as status is re-quested each time the player is selected
// in the FormationCoach workflows
// </summary>
// ***********************************************************************
using Microsoft.Extensions.Caching.Memory;

namespace ClubStat.Infrastructure.Factories
{
    public interface IPlayerMentainance
    {
        Task<PlayerGameMotivation?> GetGameMotivationAsync(PlayerGameMotivation model);
        Task SavePlayerMotivation(PlayerGameMotivation model);
    }

    internal class PlayerMentainance : ApiBasedFactory, IPlayerMentainance
    {
        private readonly IMemoryCache _memory;
        private readonly LoggedInUser _loggedInUser;//service is transiant, this is always the current player

        public PlayerMentainance(IConfiguration configuration, IHttpClientFactory clientFactory, IMemoryCache memory, LoggedInUser loggedInUser)
            : base(configuration, clientFactory)
        {
            _memory = memory;
            _loggedInUser = loggedInUser;
        }

        public async Task<PlayerGameMotivation?> GetGameMotivationAsync(PlayerGameMotivation model)
        {
            if (_loggedInUser.UserType == UserType.Coach
                && _memory.TryGetValue<PlayerGameMotivation>(GetKey(model), out var gameMotivation)
                && gameMotivation is not null)
            {
                return gameMotivation;
            }

            var answer = await base.PostAsync<PlayerGameMotivation>(MagicStrings.GetPlayerMotivation, model);
            if (answer is not null && _loggedInUser.UserType == UserType.Coach)
            {
                _memory.Set(GetKey(model), answer);
            }
            return answer;
        }
        public async Task SavePlayerMotivation(PlayerGameMotivation model)
        {
           var answer = await base.WriteDataAsync(MagicStrings.SavePlayerMotivation, model).ConfigureAwait(false);
            if (_loggedInUser.UserType == UserType.Coach)
            {
                _memory.Set(GetKey(model), model);
            }
            
        }

        static string GetKey(PlayerGameMotivation model) => $"PlayerMotivation-{model.PlayerId}-{model.MatchId}";
    }
}
