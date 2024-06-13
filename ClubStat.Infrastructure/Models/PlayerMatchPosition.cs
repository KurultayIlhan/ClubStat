// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 01-Jun-2024
//
// Last Modified By : Ilhan
// Last Modified On : Tue 11-Jun-2024
// ***********************************************************************
// <copyright file="PlayerMatchPosition.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;

using System.Data.SqlTypes;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{
    [JsonSerializable(typeof(PlayerMatchPosition))]
    [JsonSourceGenerationOptions(
                GenerationMode = JsonSourceGenerationMode.Metadata,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
    )]
    public partial class PlayerMatchPositionJsonContext : JsonSerializerContext
    {
    }
    [JsonSerializable(typeof(List<PlayerMatchPosition>))]
    [JsonSourceGenerationOptions(
                GenerationMode = JsonSourceGenerationMode.Metadata,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
    )]
    public partial class PlayerMatchPositionListJsonContext : JsonSerializerContext
    {
    }
    public partial class PlayerMatchPosition : ObservableObject, IAsJson
    {
        private int _matchId;
        private Guid _playerId;
        private MatchPosition _position;
        private DateTime _onFieldUtc;
        private DateTime? _offFieldUtc;
        public PlayerMatchPosition()
        {

        }

        public int MatchId { get => _matchId; set => SetProperty(ref _matchId, value); }
        public Guid PlayerId { get => _playerId; set => SetProperty(ref _playerId, value); }

        public MatchPosition Position { get => _position; set => SetProperty(ref _position, value); }

        
        public DateTime OnFieldUtc { get => _onFieldUtc; set => SetProperty(ref _onFieldUtc, value); }

        
        public DateTime? OffFieldUtc { get => _onFieldUtc; set => SetProperty(ref _offFieldUtc, value); }
        
        
        public string AsJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, PlayerMatchPositionJsonContext.Default.PlayerMatchPosition);
        }
    }
}
