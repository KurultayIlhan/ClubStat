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
/// Interface IFromJson will generate a type in a controlled way from json string
/// </summary>
/// <remarks>
/// The interface abstracts away the need to know details of a class when converting it from json
/// </remarks>
internal abstract class TFromJson<T>
{
    public abstract T FromJson(string json);

}

