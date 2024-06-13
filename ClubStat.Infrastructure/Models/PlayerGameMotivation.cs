// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Thu 16-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Fri 24-May-2024
// ***********************************************************************
// <copyright file="PlayerMovent.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{
    [JsonSerializable(typeof(PlayerGameMotivation))]
    [JsonSourceGenerationOptions(
GenerationMode = JsonSourceGenerationMode.Metadata,
DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
PropertyNameCaseInsensitive = true,
WriteIndented = true
)]
    public partial class PlayerGameMotivationJsonContext : JsonSerializerContext
    {
    }

    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public sealed class PlayerGameMotivation: IAsJson, IEquatable<PlayerGameMotivation?>
    {
        [JsonConstructor]
        public PlayerGameMotivation(Guid playerId, int matchId, int playerAttitude, int playerMotivation)
        {
            PlayerId = playerId;
            MatchId = matchId;
            PlayerAttitude = playerAttitude;
            PlayerMotivation = playerMotivation;
        }
        [Required]
        public Guid PlayerId { get; }
        [Required]
        [Range(1,int.MaxValue)]
        public int MatchId { get; }
        [Required]
        [Range(0,10)]
        public int PlayerAttitude { get; set; }
        [Required]
        [Range(0,10)]
        public int PlayerMotivation { get; set; }

        public override string ToString()
        {
            return $"{PlayerId} on Match{MatchId} scored a attitude of {PlayerAttitude}/10 and a Motivation of {PlayerMotivation}/10";
        }
        public string AsJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, PlayerGameMotivationJsonContext.Default.PlayerGameMotivation);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as PlayerGameMotivation);
        }

        public bool Equals(PlayerGameMotivation? other)
        {
            return other is not null &&
                   PlayerId.Equals(other.PlayerId) &&
                   MatchId == other.MatchId &&
                   PlayerAttitude == other.PlayerAttitude &&
                   PlayerMotivation == other.PlayerMotivation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerId, MatchId, PlayerAttitude, PlayerMotivation);
        }

        public static bool operator ==(PlayerGameMotivation? left, PlayerGameMotivation? right)
        {
            return EqualityComparer<PlayerGameMotivation>.Default.Equals(left, right);
        }

        public static bool operator !=(PlayerGameMotivation? left, PlayerGameMotivation? right)
        {
            return !(left == right);
        }

        private string GetDebuggerDisplay()
        {
            return $"{PlayerId}: Attitude:{PlayerAttitude}/10 Motivation:{PlayerMotivation}/10";
        }
    }

}
