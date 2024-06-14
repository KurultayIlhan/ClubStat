// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Thu 06-Jun-2024
// ***********************************************************************
// <copyright file="StatsPlayerViewModel.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ClubStatUI.ViewModels
{
    public partial class StatsPlayerViewModel : ObservableObject, ILoadAsync
    {
        private readonly IPlayerRecorder _playerRecorder;
        private readonly IMatchFactory _matchFactory;

        private readonly CurrentGameViewModel _currentGame;
        private readonly IMessageDialog _messageDialog;
        [ObservableProperty]
        private PlayerMotionStatistics _statistics = new();
        [ObservableProperty]
        private Match? _lastMatch;

        [ObservableProperty]
        Player? _user;

        [ObservableProperty]
        PlayerMovement? _playerMovement;

        [ObservableProperty]
        ObservableCollection<PlayerMovement>? _locations;

        public StatsPlayerViewModel(IPlayerRecorder playerRecorder, ILoginFactory loginFactory, IMatchFactory matchFactory
            , CurrentGameViewModel currentGame, IMessageDialog messageDialog)
        {
            _playerRecorder = playerRecorder;
            _matchFactory = matchFactory;
            _currentGame = currentGame;
            _messageDialog = messageDialog;
            if (loginFactory.CurrentUser is Player player)
            {
                User = player;
            }
            _currentGame.PropertyChanged += CopyLocation;
        }



        public CurrentGameViewModel CurrentGame => _currentGame;

        public async Task ExecuteAsync()
        {
            if (User is not null)
            {
                var match = await _matchFactory.GetPlayersLastMatch(User.UserId).ConfigureAwait(true);
                if (match is not null)
                {
                    LastMatch = match;

                    var data = await _playerRecorder.GetPlayerMotionStatisticsAsync(User, LastMatch.MatchId).ConfigureAwait(true);
                    if (data != null)
                    {
                        Statistics = data;
                    }
                    var live= await _playerRecorder.GetPlayerMovementsAsync(player:User,match:match).ConfigureAwait(true);
                    if (live is not null)
                    {
                        Locations =new(live);
                    }

                }
            }
            await CurrentGame.ExecuteAsync().ConfigureAwait(true);
        }
        [RelayCommand]
        private async Task OnStart()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>().ConfigureAwait(true);
            if (status != PermissionStatus.Granted)
            {
                // Request the permissions as it is not already granted
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>().ConfigureAwait(true);

            }
            if (status == PermissionStatus.Granted)
            {                        
                CurrentGame.Start();
                //why delay?
                //await Task.Delay(100);
            }
            else
            { 
                _messageDialog.ShowMessage("Locatietoestemming is vereist om het spel op te nemen.");
            }

        }

        [RelayCommand]
        private void OnStop()
        {
            CurrentGame.Stop();
        }

        private void CopyLocation(object? sender, PropertyChangedEventArgs e)
        {
           
            if ( e.PropertyName == nameof(CurrentGameViewModel.CurrentLocation))
            {
                if (_currentGame.CurrentLocation is null) return;

                var copyFrom= _currentGame.CurrentLocation;
                var calculatedLat = copyFrom.Lat / 10;
                var calculatedLng = copyFrom.Lng / 10;
                var newItem = new PlayerMovement()
                {                 
                    Location = $"{calculatedLat} - {calculatedLng}",
                    RecordedUtc = copyFrom.Recorded
                };;
                Locations ??= new();
                Locations.Add(newItem);

            }
        }
    }
}
