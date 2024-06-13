// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : Sat 01-Jun-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Sat 01-Jun-2024
// ***********************************************************************
// <copyright file="PlayerActivityRow.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{
    [JsonSerializable(typeof(PlayerActivityRow))]
    [JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
    public partial class PlayerActivityRowJsonContext : JsonSerializerContext { }
    
    
    [JsonSerializable(typeof(List<PlayerActivityRow>))]
    [JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
    public partial class PlayerActivityRowsJsonContext : JsonSerializerContext { }

    /// <summary>
    /// Class PlayerActivityRow.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public sealed class PlayerActivityRow : IAsJson
    {
        private DateTime _recordedUtc;

        /// <summary>
        /// Gets or sets the recorded UTC.
        /// </summary>
        /// <value>The recorded UTC.</value>
        [Required]        
        public DateTime RecordedUtc 
        { 
            get => _recordedUtc;
            set=>  _recordedUtc = value;
                
            
        }
        
        

        /// <summary>
        /// Gets or sets the player identifier.
        /// </summary>
        /// <value>The player identifier.</value>
        [Required]
        public Guid PlayerId { get; set; }
        /// <summary>
        /// Gets or sets the Match identifier.
        /// </summary>
        /// <value>The Match identifier.</value>
        [Required]
        [Range(1, int.MaxValue)]
        public int MatchId { get; set; }
        /// <summary>
        /// Gets or sets the player club identifier.
        /// </summary>
        /// <value>The player club identifier.</value>
        [Required]
        [Range(1, int.MaxValue)]
        public int PlayerClubId { get; set; }

        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        /// <value>The activity.</value>
        [JsonConverter(typeof(JsonStringEnumConverter<PlayerActivities>))]
        public PlayerActivities Activity { get; set; }

        public override string ToString()
        {
            return $"{PlayerId}:{Activity} on {RecordedUtc} (vaid:{IsValid(out _)})";
        }
        string IAsJson.AsJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, PlayerActivityRowJsonContext.Default.PlayerActivityRow);
        }

        public bool IsValid(out string validationError)
        {
            if (RecordedUtc <= SqlDateTime.MinValue.Value)
            {
                validationError = "RecordedUtc cannot be the default value.";
                return false;
            }

            if (PlayerId == Guid.Empty)
            {
                validationError = "PlayerId cannot be the default value.";
                return false;
            }

            if (MatchId == default)
            {
                validationError = "MatchId cannot be the default value.";
                return false;
            }

            if (PlayerClubId == default)
            {
                validationError = "PlayerClubId cannot be the default value.";
                return false;
            }

            if (Activity == PlayerActivities.None)
            {
                validationError = "Activity cannot be the default value.";
                return false;
            }

            validationError = string.Empty;
            return true;
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
