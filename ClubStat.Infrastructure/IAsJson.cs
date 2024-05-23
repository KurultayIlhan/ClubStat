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
/// Interface IAsJson will generate a controlled json string from the type
/// </summary>
/// <remarks>
/// The interface abstracts away the need to know details of a class when converting it to json
/// </remarks>
public interface IAsJson
{
    string AsJson();

}

