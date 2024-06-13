// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : Thu 16-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Sat 01-Jun-2024
// ***********************************************************************
// <copyright file="PlayerRecorder.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStat.Infrastructure.Factories
{
    public interface IPlayerRecorder
    {
        /// <summary>
        /// Gets the last update location during a active game.
        /// </summary>
        /// <value>The last update.</value>
        public DateTime LastUpdate { get; }

        /// <summary>
        /// Gets the last rest API upload error during a active game..
        /// </summary>
        /// <value>The last rest error.</value>
        public DateTime? LastRestError { get; }
        /// <summary>
        /// Records the activity of a player.
        /// </summary>
        /// <remarks><see cref="PlayerActivities.None"/>indicates he entered the field</remarks>
        /// <param name="player">The player.</param>
        /// <param name="match">The match.</param>
        /// <param name="activity">The activity.</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        Task RecordActivity(Player player, Match match, PlayerActivities activity);

        /// <summary>
        /// Records the location of the player on the game.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="match">The match.</param>
        /// <param name="location">The location.</param>
        void RecordLocation(Player player, Match match, PlayerDynamicsLocation location);

        /// <summary>
        /// Uploads the locations asynchronous and save the location to the API website.
        /// </summary>
        /// <remarks>
        /// <see cref="LastRestError"/> for any issues during uploads
        /// </remarks>
        /// <returns>Task</returns>
        Task UploadLocationsAsync();

        //get the locations back from the website
        Task<List<PlayerMovement>?> GetPlayerMovementsAsync(Player player, Match match);
        /// <summary>
        /// Gets the players last known location.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="match">The match.</param>
        /// <returns>Task&lt;System.Nullable&lt;PlayerMovement&gt;&gt;.</returns>
        Task<PlayerMovement?> GetPlayersLastKnownLocation(Player player, Match match);
        /// <summary>
        /// Gets the teams last known location.
        /// </summary>
        /// <param name="clubId">The club to query.</param>
        /// <param name="matchId">The match ID to query.</param>
        /// <returns>Task&lt;System.Nullable&lt;PlayerMovement&gt;&gt;.</returns>
        Task<List<PlayerMovement>?> GetTeamsLastKnownLocation(int clubId, int matchId);
        /// <summary>
        /// Gets the player statistics.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="afterDate">The after date.</param>
        /// <returns>Task&lt;PlayerStatistics&gt;.</returns>
        Task<PlayerStatistics> GetPlayerStatisticsAsyc(Player player, DateTime afterDate);
        /// <summary>
        /// Gets the player statistics for the current season.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>Task&lt;PlayerStatistics&gt;.</returns>
        Task<PlayerStatistics> GetPlayerStatisticsAsync(Player player);
        Task<PlayerMotionStatistics> GetPlayerMotionStatisticsAsync(Player user, int matchId);

        /// <summary>
        /// Gets the record activity for a player and a match.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="match">The match.</param>
        /// <returns>Task&lt;List&lt;PlayerActivityRow&gt;&gt;.</returns>
        Task<List<PlayerActivityRow>> GetRecordActivity(Player player, Match match);
    }
}
