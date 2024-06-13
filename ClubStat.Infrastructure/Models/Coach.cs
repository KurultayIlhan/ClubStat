// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : 02-11-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : 02-11-2024
// ***********************************************************************
// <copyright file="Player.cs" company="ClubStat.Infrastructure">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// Task 9: Add player endpoint and allow it to retrieve player data. update the Payer class to reflect figma property
// </summary>
// ***********************************************************************
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models;

[JsonSerializable(typeof(Coach))]
[JsonSourceGenerationOptions(
                GenerationMode = JsonSourceGenerationMode.Metadata,
                //  Converters = [typeof(GDPRObfuscatedStringConverter)],
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
    )]
public partial class CoachJsonContext : JsonSerializerContext
{
}
public partial class Coach : ClubMember
{
    int _playersLeague;
    char _playersLeagueLevel = 'A';
    byte _coachLevel;

    public int PlayersLeague { get => _playersLeague; set => SetProperty(ref _playersLeague, value); }
    public char PlayersLeagueLevel { get => _playersLeagueLevel; set => SetProperty(ref _playersLeagueLevel, value); }
    public byte CoachLevel { get => _coachLevel; set => SetProperty(ref _coachLevel, value); }
}
