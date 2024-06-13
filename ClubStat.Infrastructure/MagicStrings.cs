// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Sun 12-May-2024
// ***********************************************************************
// <copyright file="MagicStrings.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary>
// strings used by the application
// </summary>
// ***********************************************************************
using System.Reflection.Metadata;

namespace ClubStat.Infrastructure
{
    public static class MagicStrings
    {

        const string _dashboardApi = "api/dashboard/{0}}/{1}";
        const string _playerApi = "api/Player/{0}";
        const string _coachApi = "api/Coach/{0}";
        const string _playerProfileApi = "api/Player/ProfileImage/{0}";

        /// <summary>
        /// The login API endpoint
        /// </summary>
        public const string LoginApi = "api/login";
        public const string PlayerNextMatch = "api/player/nextmatch";
        public const string PlayerPostPicture = "api/Player/ProfileImage";
        public const string RecordPlayerLocation = "api/Player/SavePlayerRecording";
        public const string SavePlayerMotivation = "api/Player/SavePlayerMotivation";
        public const string GetPlayerMotivation = "api/Player/GetPlayerMotivation";
        public const string PlayerRecordActivityUrl = "api/player/recordActivity";
        public const string RecordPlayerMatchPosition="api/match/PlayerPositions";
        /// <summary>
        /// The API header field that the server will look for
        /// </summary>
        public const string API_HEADER = "X-API";
        

        /// <summary>
        /// Helper Method to get the Dashboards for a user.
        /// </summary>
        /// <param name="user">The user to query.</param>
        /// <returns>API endpoint</returns>
        public static string DashboardUrlFor(this IAuthenticatedUser user)
        {
            if (user.UserType == UserType.Player || user.UserType == UserType.Coach || user.UserType == UserType.Delegee)
            {
                return string.Format(_dashboardApi, user.UserType, user.UserId);
            }

            throw new NotImplementedException($"{user.UserType} is not a valid option, user must have been authenticated.");
        }

        internal static string PlayerUrl(Guid userId)
        {
            return string.Format(_playerApi, userId);
        }

        internal static string CoachUrl(Guid userId)
        {
            return string.Format(_coachApi, userId);
        }
        internal static string PlayerProfileUrl(Guid userId)
        {
            return string.Format(_playerProfileApi, userId);
        }

        internal static string GetPlayerMovementsInGame(Guid userId, int matchId)
        {
            return $"api/player/acivity{userId}/{matchId}";
        }
        internal static string GetPlayerStatistics(Guid userId, DateTime afterDate)
        {
            return $"api/player/Statistics/{userId}/{afterDate.ToUniversalTime().Ticks}";
        }

    }
}
