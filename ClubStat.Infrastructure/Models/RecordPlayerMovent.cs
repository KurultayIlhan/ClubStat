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
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{
    [JsonSerializable(typeof(RecordPlayerMovent))]
    [JsonSourceGenerationOptions(
                GenerationMode = JsonSourceGenerationMode.Metadata,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,       
        
                WriteIndented = true
        )]
    public partial class RecordPlayerMoventJsonContext : JsonSerializerContext
    {
    }

    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class RecordPlayerMovent:IAsJson, IEquatable<RecordPlayerMovent?>
    {
        

        [JsonConstructor]
        public RecordPlayerMovent()
        {
            
        }
        public RecordPlayerMovent(Guid playerId,int matchId,double latitude, double longitude, DateTime recordedUtc)
        {
            PlayerId = playerId;
            MatchId = matchId;
            Latitude = latitude;
            Longitude = longitude;
            RecordedUtc = recordedUtc;
        }

        public Guid PlayerId { get; set; }
        public int MatchId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime RecordedUtc { get; set; }


        public string AsJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, RecordPlayerMoventJsonContext.Default.RecordPlayerMovent);
        }

        override public string ToString()
        {
            return $"{RecordedUtc}:{ Latitude}/{Longitude}";
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as RecordPlayerMovent);
        }

        public bool Equals(RecordPlayerMovent? other)
        {
            return other is not null &&
                   PlayerId.Equals(other.PlayerId) &&
                   MatchId == other.MatchId &&
                   Latitude == other.Latitude &&
                   Longitude == other.Longitude &&
                   RecordedUtc == other.RecordedUtc;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerId, MatchId, Latitude, Longitude, RecordedUtc);
        }

        public static bool operator ==(RecordPlayerMovent? left, RecordPlayerMovent? right)
        {
            return EqualityComparer<RecordPlayerMovent>.Default.Equals(left, right);
        }

        public static bool operator !=(RecordPlayerMovent? left, RecordPlayerMovent? right)
        {
            return !(left == right);
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }

}
