// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : 02-11-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Wed 05-Jun-2024
// ***********************************************************************
// <copyright file="Player.cs" company="ClubStat.Infrastructure">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>Task 9: Add player endpoint and allow it to retrieve player data. update the Payer class to reflect figma property</summary>
// ***********************************************************************
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models;

[JsonSerializable(typeof(Player))]
[JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
public partial class PlayerJsonContext : JsonSerializerContext
{
}

[JsonSerializable(typeof(List<Player>))]
[JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
public partial class TeamOfPlayerJsonContext : JsonSerializerContext
{
}

/// <summary>
/// Basic player data
/// </summary>
public sealed class Player : ClubMember, IAuthenticatedUser, IAsJson, IEquatable<Player?>
{


    string? _firstName = null;
    string? _lastName = null;
    DateTime _dateOfBirth;
    char _playersLeagueLevel = 'A';
    int _playersLeague;
    int? _biologicAge;

    int _playerAttitude;
    int _playerMotivation;
    int _playerNumber = Random.Shared.Next(1,99);//temprary

    [JsonIgnore]
    public double ScaledPlayerAttitude => PlayerAttitude / 10.0;

    // Scale the PlayerMotivation to a range of 0 to 10
    [JsonIgnore]
    public double ScaledPlayerMotivation => PlayerMotivation / 10.0;

    public Player()
    {

    }
    [JsonIgnore]
    public int PlayerNumber 
    {
        get => _playerNumber;
        set => SetProperty(ref _playerNumber , value);
    }

    [JsonIgnore]
    public int Age
    {
        get
        {

            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;

            // Adjust age when today is before the birth date this year
            if (DateOfBirth > today.AddYears(-age)) age--;

            // Special handling for February 29th birthdays
            if (DateOfBirth.Month == 2 && DateOfBirth.Day == 29)
            {
                // Check if the current year is a leap year
                if (!DateTime.IsLeapYear(today.Year))
                {
                    // If today's date is before March 1st and the current year is not a leap year,
                    // adjust the birthday to February 28th of the current year for age calculation
                    if (today < new DateTime(today.Year, 3, 1))
                        age--;
                }
            }

            return age;

        }
    }
    /// <summary>
    /// Helpermethod that Gets the league.
    /// </summary>
    /// <value>The league as U12B.</value>
    [JsonIgnore]
    public string League
    {
        get => string.Concat('U', PlayersLeague, PlayersLeagueLevel);
    }
    [Required(AllowEmptyStrings = false)]
    public new string FullName
    {
        get
        {
            if (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName))
            {
                return $"{FirstName} {LastName}";
            }
            else
            {
                return base.FullName;
            }
        }
        set
        {
            base.FullName = value;
            base.OnPropertyChanged(nameof(FullName));

        }
    }

    public int PlayerMotivation { get => _playerMotivation; set => SetProperty(ref _playerMotivation, value); }
    public int PlayerAttitude { get => _playerAttitude; set => SetProperty(ref _playerAttitude, value); }

    public int? BiologicAge { get => _biologicAge; set => SetProperty(ref _biologicAge, value); }
    public int PlayersLeague { get => _playersLeague; set => SetProperty(ref _playersLeague, value); }
    public char PlayersLeagueLevel { get => _playersLeagueLevel; set => SetProperty(ref _playersLeagueLevel, value); }

    
    public DateTime DateOfBirth 
    { 
        get => _dateOfBirth;
        set => SetProperty(ref _dateOfBirth, value);
            
        
    }
    
    public string? LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
    public string? FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }

    public string AsJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, PlayerJsonContext.Default.Player);
    }
    public override string ToString()
    {
        return $"{FullName} - ({PlayerNumber})";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Player);
    }

    public bool Equals(Player? other)
    {
        return other is not null &&
               FullName == other.FullName &&
               UserType == other.UserType &&
               UserId.Equals(other.UserId) &&
               ClubId == other.ClubId &&
               ScaledPlayerAttitude == other.ScaledPlayerAttitude &&
               ScaledPlayerMotivation == other.ScaledPlayerMotivation &&
               Age == other.Age &&
               League == other.League &&
               FullName == other.FullName &&
               PlayerMotivation == other.PlayerMotivation &&
               PlayerAttitude == other.PlayerAttitude &&
               BiologicAge == other.BiologicAge &&
               PlayersLeague == other.PlayersLeague &&
               PlayersLeagueLevel == other.PlayersLeagueLevel &&
               DateOfBirth == other.DateOfBirth &&
               LastName == other.LastName &&
               FirstName == other.FirstName;
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(FullName);
        hash.Add(UserType);
        hash.Add(UserId);
        hash.Add(ClubId);
        hash.Add(ScaledPlayerAttitude);
        hash.Add(ScaledPlayerMotivation);
        hash.Add(Age);
        hash.Add(League);
        hash.Add(FullName);
        hash.Add(PlayerMotivation);
        hash.Add(PlayerAttitude);
        hash.Add(BiologicAge);
        hash.Add(PlayersLeague);
        hash.Add(PlayersLeagueLevel);
        hash.Add(DateOfBirth);
        hash.Add(LastName);
        hash.Add(FirstName);
        return hash.ToHashCode();
    }

    public static bool operator ==(Player? left, Player? right)
    {
        return EqualityComparer<Player>.Default.Equals(left, right);
    }

    public static bool operator != (Player? left, Player? right)
    {
        return !(left == right);
    }
}
