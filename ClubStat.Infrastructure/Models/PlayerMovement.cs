// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Thu 16-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Fri 24-May-2024
// ***********************************************************************
// <copyright file="PlayerMovement.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;

using System.Data.SqlTypes;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{

    [JsonSerializable(typeof(List<PlayerMovement>))]
    [JsonSourceGenerationOptions(
        GenerationMode = JsonSourceGenerationMode.Metadata,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNameCaseInsensitive = true,
        IgnoreReadOnlyFields = true,
        WriteIndented = true
)]
    public partial class PlayerMovementsJsonContext : JsonSerializerContext { }

    [JsonSerializable(typeof(PlayerMovement))]
    [JsonSourceGenerationOptions(
        GenerationMode = JsonSourceGenerationMode.Metadata,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNameCaseInsensitive = true,
        IgnoreReadOnlyFields = true,        
        WriteIndented = true
)]
    public partial class PlayerMovementJsonContext : JsonSerializerContext
    {
    }
    /// <summary>
    /// Class PlayerMovement is populated based on a player and can be used to read the location and speed of a player during a match.
    /// </summary>
    public partial class PlayerMovement:ObservableObject
    {
        private string _location = "0 - 0";
        private double? _speed;
        private double? _direction;

        /// <summary>
        /// Gets or sets the match identifier.
        /// </summary>
        /// <value>The match identifier.</value>
        public int MatchId { get; set; }
        /// <summary>
        /// Gets or sets the player identifier.
        /// </summary>
        /// <value>The player identifier.</value>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the recorded movement in UTC.
        /// </summary>
        /// <value>The recorded date time UTC.</value>
        public DateTime RecordedUtc{ get; set; }


        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <remarks>
        /// Formal is {lat} - {long}
        /// </remarks>
        /// <value>The location.</value>
        public required string Location
        {
            get => _location;
            set
            {
                if (SetProperty(ref _location, value))
                { 
                    base.OnPropertyChanged(nameof(Latitude));
                    base.OnPropertyChanged(nameof(Longitude));
                }
            }
        } // Assuming Location is stored as a string, modify this as needed

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>The speed.</value>
        public double? Speed { get => _speed; set =>SetProperty(ref _speed , value); }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public double? Direction { get => _direction; set =>SetProperty(ref _direction , value); }

        [JsonIgnore]
        public decimal Latitude
        {
            get
            {
                var ix = Location.IndexOf('-');
                if (ix >= 0 && decimal.TryParse(Location[0..( ix- 1)], out var value))
                    return value;

                return 0;
            }
        }

        [JsonIgnore]
        public decimal Longitude
        {
            get
            {
                var ix = Location.IndexOf('-');
                if (ix > 0 && decimal.TryParse(Location[(ix + 1)..], out var value))
                {
                    return value;
                }
                return 0;
            }
        }
    }
}
