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
// <summary></summary>
// ***********************************************************************
namespace ClubStat.Infrastructure;

/// <summary>
/// Interface IAuthenticatedUser is used to communicate with the API layer  
/// </summary>
/// <remarks>
/// As we use Observable object we can't create a base class without introducing un necessary concerns
/// The solution is a interface that states the type the user is
/// </remarks>
public interface IAuthenticatedUser
{
    UserType UserType { get; }
    string FullName { get; }
    Guid UserId { get; }
}

