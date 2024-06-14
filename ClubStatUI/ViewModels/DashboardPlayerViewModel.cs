// ***********************************************************************
// Assembly         : ClubStatUI
// Author           : Ilhan Kurultay
// Created          : Mon 12-Feb-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Tue 20-Feb-2024
// ***********************************************************************
// <copyright file="DashboardPlayerViewModel.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

using Plugin.LocalNotification;
using Walter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubStatUI.ViewModels
{
    
    public partial class DashboardPlayerViewModel: ObservableObject, ILoadAsync
    {
        private readonly IPlayerRecorder _playerRecorder;
        private readonly IMatchFactory _matchFactory;
        private readonly ILoginFactory _loginFactory;
        [ObservableProperty]
        private string _imagePath ="bronze.png";

        [ObservableProperty]
        ClubStat.Infrastructure.Models.Player? _user;

        [ObservableProperty]
        private PlayerMotionStatistics _statistics = new();

        [ObservableProperty]
        UpcomingMatchView _upcomingMatch;

        [ObservableProperty]
        PersonalStatisticsView? _personalStatistics;

        [ObservableProperty]
        TrainingResults? _trainingResults;

        [ObservableProperty]
        PlayerOfTheMonthView? _playerOfTheMonth;

        [ObservableProperty]
        private Match? _lastMatch;

        [ObservableProperty]
        Player? _loggedInUser;

        public DashboardPlayerViewModel(IPlayerRecorder playerRecorder, IMatchFactory matchFactory, ILoginFactory loginFactory)
        {
            // TODO: navigate to login when current user is null
            if (loginFactory.CurrentUser is Player player) 
            { 
                User = player;
            }

            // Initialize other fields with required arguments
            _upcomingMatch = new UpcomingMatchView();
            _personalStatistics = new PersonalStatisticsView();

            // Provide required arguments for PlayerOfTheMonthView and TrainingResults constructors
            _trainingResults = new TrainingResults(new ScoreZeroToFive(), new ScoreZeroToFive());
            _playerOfTheMonth = new PlayerOfTheMonthView(new Team(), new Player());
            _loggedInUser = new Player();
            _playerRecorder = playerRecorder;
            _matchFactory = matchFactory;
            _loginFactory = loginFactory;
        }
        



        public async Task ExecuteAsync()
        {
            if ((User is Player player))
            {
                UpcomingMatch.Division = player.PlayersLeagueLevel.ToDivision();
                
            }
            if (User is not null)
            {
                var match = await _matchFactory.GetPlayersLastMatch(User.UserId).ConfigureAwait(true);
                if (match is not null)
                {
                    LastMatch = match;

                    var data = await _playerRecorder.GetPlayerMotionStatisticsAsync(User, LastMatch.MatchId).ConfigureAwait(true);
                    if (data != null)
                    {
                        if(data.Sprints > 500 && data.AverageSpeed > 3)
                        {
                            ImagePath = "gold.png";
                        } 
                        else if(data.Sprints > 100 && data.AverageSpeed > 0)
                        {
                            ImagePath = "silver.png";
                        }
                        else
                        {
                            ImagePath = "bronze.png";
                        }
                   
                        Statistics = data;
                    }
                }
            }


            var id = User?.UserId ?? _loginFactory.CurrentUser?.UserId;
            if (id.HasValue)
            {
                var match = await _matchFactory.GetPlyersNextMatch(playerId: id.Value).ConfigureAwait(true);
                if (match is not null)
                {
                    base.OnPropertyChanging(nameof(UpcomingMatch));
                    UpcomingMatch.Match = match;
                    base.OnPropertyChanged(nameof(UpcomingMatch));
                }
            }
            if(UpcomingMatch.Match is not null) 
            {
                var canNotify = await LocalNotificationCenter.Current.AreNotificationsEnabled().ConfigureAwait(true);
                if (canNotify)
                {
                    var list = await LocalNotificationCenter.Current.GetDeliveredNotificationList().ConfigureAwait(true);

                    UpcomingMatch.RemindMe = list.FirstOrDefault(f => f.NotificationId == UpcomingMatch.Match.GetReminderId()) != null;
                    base.OnPropertyChanged(nameof(UpcomingMatch));
                }

            }
        }

        public void SetReminder()
        {
            if(!LocalNotificationCenter.Current.IsSupported)
            if (UpcomingMatch.Match is null) return;

            var notification = new NotificationRequest
            {
                NotificationId = UpcomingMatch.Match!.GetReminderId(), // Ensure unique ID for the match
                Title = "Upcoming Match Reminder",
                Description = $"Match between {UpcomingMatch.Match.HomeTeam.ClubName} and {UpcomingMatch.Match.AwayTeam.ClubName} is starting soon.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = UpcomingMatch.Match.MatchDate.AddMinutes(-15) // Notify 15 minutes before the match
                }
            };

            LocalNotificationCenter.Current.Show(notification);
            UpcomingMatch.RemindMe = true;
        }
    }
}
