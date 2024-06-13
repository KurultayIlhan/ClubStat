// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Thu 16-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Thu 16-May-2024
// ***********************************************************************
// <copyright file="Club.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;

using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{
    [JsonSerializable(typeof(Club))]
    [JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
    public partial class ClubJsonContext : JsonSerializerContext
    {
    }

    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public partial class Club : ObservableObject, IAsJson, IEquatable<Club?>
    {
        int _clubId;
        string _clubName;
        string _clubIconUrl;
        string _clubCity;

        [JsonConstructor]
        public Club(int clubId, string clubName, string clubIconUrl, string clubCity)
        {
            _clubCity = clubCity;
            _clubId = clubId;
            _clubIconUrl = clubIconUrl;
            _clubName = clubName;

        }

        public int ClubId { get => _clubId; set =>SetProperty(ref _clubId , value); }
        public string ClubIconUrl { get => _clubIconUrl; set => SetProperty(ref _clubIconUrl , value); }
        public string ClubCity { get => _clubCity; set => SetProperty(ref _clubCity , value); }
        public string ClubName { get => _clubName; set =>SetProperty(ref _clubName , value); }

        public string AsJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, ClubJsonContext.Default.Club);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Club);
        }

        public bool Equals(Club? other)
        {
            return other is not null &&
                   _clubId == other._clubId &&
                   _clubName == other._clubName &&
                   _clubIconUrl == other._clubIconUrl &&
                   _clubCity == other._clubCity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_clubId, _clubName, _clubIconUrl, _clubCity);
        }

        public static bool operator ==(Club? left, Club? right)
        {
            return EqualityComparer<Club>.Default.Equals(left, right);
        }

        public static bool operator !=(Club? left, Club? right)
        {
            return !(left == right);
        }
        /// <summary>
        /// Gets the debugger display name for easy debugging.
        /// </summary>
        /// <returns>Clib Id with name and city.</returns>
        private string GetDebuggerDisplay()
        {
            return $"{ClubId}:{ClubName} - ({ClubCity})";
        }
    }
}
