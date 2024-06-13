// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : 02-20-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : 02-20-2024
// ***********************************************************************
// <copyright file="UserType.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary>User profile indicator</summary>
// ***********************************************************************

namespace ClubStat.Infrastructure.Models;


/// <summary>
/// Shows the user which profile he will see
/// </summary>
public enum UserType
{
    /// <summary>
    /// If the usertype is 0, it's an error and will get send back to the login
    /// </summary>
    None = 0,
    /// <summary>
    /// The player
    /// </summary>
    Player = 1,
    /// <summary>
    /// The coach
    /// </summary>
    Coach = 2,
    /// <summary>
    /// The delegee
    /// </summary>
    Delegee = 3
}
