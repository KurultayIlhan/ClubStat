// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : erte0
// Created          : 02-11-2024
//
// Last Modified By : erte0
// Last Modified On : 02-11-2024
// ***********************************************************************
// <copyright file="Player.cs" company="ClubStat.Infrastructure">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// Task 9: Add player endpoint and allow it to retrieve player data. update the Payer class to reflect figma property
// </summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;

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

/// <summary>
/// Basic player data
/// </summary>
public partial class Player : LoggedInUser, IAuthenticatedUser, IAsJson
{
    [ObservableProperty]
    string? _firstName = null;

    [ObservableProperty]
    string? _lastName = null;


    [ObservableProperty]
    DateTime _dateOfBirth;

    [ObservableProperty]
    char _playersLeagueLevel = 'A';

    [ObservableProperty]
    int _playersLeague;

    [ObservableProperty]
    int? _biologicAge;

    [ObservableProperty]
    int _clubId;

    [ObservableProperty]
    int _playerAttitude;

    [ObservableProperty]
    int _playerMotivation;



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
    public string League
    {
        get => string.Concat('U', PlayersLeague, PlayersLeagueLevel);
    }

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

        }
    }



    public string AsJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(this, PlayerJsonContext.Default.Player);
    }
}
