// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Tue 14-May-2024
// ***********************************************************************
// <copyright file="DashboardPlayerView.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Factories;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ClubStat.Infrastructure.Models;

public partial class DashboardPlayerView : ObservableObject
{
    [ObservableProperty]
    UpcomingMatchView? _upcomingMatch;

    [ObservableProperty]
    PersonalStatisticsView? _personalStatistics;

    [ObservableProperty]
    TrainingResults? _trainingResults;

    [ObservableProperty]
    PlayerOfTheMonthView? _playerOfTheMonth;

    [ObservableProperty]
    Player? _loggedInUser;

    public DashboardPlayerView()
    {

    }

    public DashboardPlayerView(ILoginFactory loginFactory)
    {
        _loggedInUser = loginFactory.CurrentUser as Player;

    }
}