// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : erte0
// Created          : 02-11-2024
//
// Last Modified By : erte0
// Last Modified On : 13-05-2024
// ***********************************************************************
// <copyright file="Player.cs" company="ClubStat.Infrastructure">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// Task :
// </summary>
// ***********************************************************************
namespace ClubStat.Infrastructure.Models;

public enum PlayerActivities
{
    None = 0, 
    Assist = 1,         // double tap on the player for an assist
    Goal = 2,           // triple tap on the player for an goal
    YellowCard = 3,     // swipe left on the player for an yellow card
    RedCard = 4         // swipe right on the player for an red card
}
