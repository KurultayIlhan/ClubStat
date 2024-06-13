// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sun 12-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Fri 24-May-2024
// ***********************************************************************
// <copyright file="MagicTypes.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Text.Json.Serialization.Metadata;

namespace ClubStat.Infrastructure
{
    internal static class MagicTypes
    {



        public static JsonTypeInfo<T> JsonTypeInfo<T>()
        {
            object result = typeof(T) switch
            {
                Type t when t == typeof(LogInResult) => LogInResultJsonContext.Default.LogInResult,
                Type t when t == typeof(Player) => PlayerJsonContext.Default.Player,
                Type t when t == typeof(PlayerDynamicsLocation) => PlayerDynamicsLocationJsonContext.Default.PlayerDynamicsLocation,
                Type t when t == typeof(PlayerMovement) => PlayerMovementJsonContext.Default.PlayerMovement,
                Type t when t == typeof(PlayerGameMotivation) => PlayerGameMotivationJsonContext.Default.PlayerGameMotivation,
                Type t when t == typeof(List<PlayerMovement>) => PlayerMovementsJsonContext.Default.ListPlayerMovement,
                Type t when t == typeof(PlayerStatistics) => PlayerStatisticsJsonContext.Default.PlayerStatistics,
                Type t when t == typeof(PlayerActivityRow) => PlayerActivityRowJsonContext.Default.PlayerActivityRow,
                Type t when t == typeof(List<Match>)=>MatchAgendaJsonContext.Default.ListMatch,
                Type t when t == typeof(Coach)=>CoachJsonContext.Default.Coach,                
                
                _ => throw new NotImplementedException($"Class {typeof(T).Name} has no registered JsonContext in {nameof(MagicTypes.JsonTypeInfo)}, update this and try again")
            };

            return (JsonTypeInfo<T>)result; // Safe cast, since the cases handle the specific type checks
        }
    }
}
