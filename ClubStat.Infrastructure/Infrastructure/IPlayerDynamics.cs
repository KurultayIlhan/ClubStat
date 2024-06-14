// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Fri 24-May-2024
// ***********************************************************************
// <copyright file="IPlayerDynamics.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStat.Infrastructure;
public interface IPlayerDynamics
{
    Task<PlayerDynamicsLocation> GetPlayerDynamicsLocation();

    public bool IsInGame { get; set; }
    public LoggedInUser? Member { get; set; }
    public Match? Match { get; set; }

    public PlayerDynamicsLocation LastKnownLocation { get; }
}
