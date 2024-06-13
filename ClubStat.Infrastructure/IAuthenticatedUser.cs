// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : 02-11-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : 02-11-2024
// ***********************************************************************
// <copyright file="Player.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
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
