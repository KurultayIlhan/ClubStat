// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Thu 16-May-2024
// ***********************************************************************
// <copyright file="PlayerDynamicsLocation.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{

    [JsonSerializable(typeof(PlayerDynamicsLocation))]
    [JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
    public partial class PlayerDynamicsLocationJsonContext : JsonSerializerContext
    {
    }
    [JsonSerializable(typeof(List<PlayerDynamicsLocation>))]
    [JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
    public partial class PlayerDynamicsLocationsJsonContext : JsonSerializerContext
    {
    }

    /// <summary>
    /// Class PlayerDynamicsLocation contains the location serialized from the geo device
    /// Implements the <see cref="ClubStat.Infrastructure.IAsJson" />
    /// Implements the <see cref="System.IEquatable{ClubStat.Infrastructure.Models.PlayerDynamicsLocation}" />
    /// </summary>
    /// <seealso cref="ClubStat.Infrastructure.IAsJson" />
    /// <seealso cref="System.IEquatable{ClubStat.Infrastructure.Models.PlayerDynamicsLocation}" />
    public sealed class PlayerDynamicsLocation:IAsJson, IEquatable<PlayerDynamicsLocation?>
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDynamicsLocation"/> class.
        /// </summary>
        [JsonConstructor]
        public PlayerDynamicsLocation()
        {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerDynamicsLocation"/> class.
        /// </summary>
        /// <param name="lat">The lat.</param>
        /// <param name="lng">The LNG.</param>
        /// <param name="recorded">The recorded.</param>
        /// <param name="playerId">The player identifier.</param>
        public PlayerDynamicsLocation(decimal lat, decimal lng, DateTime recorded, int matchId,Guid playerId)
        {
            Lat = lat;
            Lng = lng;
            Recorded = recorded;
            MatchId = matchId;
            PlayerId = playerId;
        }

        /// <summary>
        /// Gets or sets the player identifier.
        /// </summary>
        /// <value>The player identifier.</value>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the lat.
        /// </summary>
        /// <value>The lat.</value>
        public decimal Lat { get; set; }
        /// <summary>
        /// Gets or sets the LNG.
        /// </summary>
        /// <value>The LNG.</value>
        public decimal Lng { get; set; }

        /// <summary>
        /// Gets or sets the match identifier.
        /// </summary>
        /// <value>The match identifier.</value>
        public int MatchId { get; set; }

        /// <summary>
        /// Gets or sets the recorded.
        /// </summary>
        /// <value>The recorded.</value>
        public DateTime Recorded { get; set; }

        /// <summary>
        /// Ases the json.
        /// </summary>
        /// <returns>System.String.</returns>
        public string AsJson()
        {
             return JsonSerializer.Serialize( this,PlayerDynamicsLocationJsonContext.Default.PlayerDynamicsLocation);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as PlayerDynamicsLocation);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(PlayerDynamicsLocation? other)
        {
            return other is not null &&
                   PlayerId.Equals(other.PlayerId) &&
                   Lat == other.Lat &&
                   Lng == other.Lng &&
                   Recorded == other.Recorded;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerId, Lat, Lng, Recorded);
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(PlayerDynamicsLocation? left, PlayerDynamicsLocation? right)
        {
            return EqualityComparer<PlayerDynamicsLocation>.Default.Equals(left, right);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(PlayerDynamicsLocation? left, PlayerDynamicsLocation? right)
        {
            return !(left == right);
        }
    }
}
