// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : 02-11-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Thu 16-May-2024
// ***********************************************************************
// <copyright file="ILoadAsync.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStat.Infrastructure;

public interface ILoadAsync
{
    Task ExecuteAsync();
}