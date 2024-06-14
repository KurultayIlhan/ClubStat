// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Fri 24-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Thu 06-Jun-2024
// ***********************************************************************
// <copyright file="CurrentGameViewModel.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

using Walter;

namespace ClubStatUI.ViewModels
{
    /// <summary>
    /// Class CurrentGameViewModel can be used in any window to show a current match player location.
    /// Implements the <see cref="CommunityToolkit.Mvvm.ComponentModel.ObservableObject" />
    /// Implements the <see cref="ILoadAsync" />
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <remarks>
    /// Can be used in a custom sharable component
    /// </remarks>
    /// <seealso cref="CommunityToolkit.Mvvm.ComponentModel.ObservableObject" />
    /// <seealso cref="ILoadAsync" />
    /// <seealso cref="System.IDisposable" />
    public partial class CurrentGameViewModel : ObservableObject, ILoadAsync, IDisposable
    {
        /// <summary>
        /// The match factory used to create match instances.
        /// </summary>
        private readonly IMatchFactory _matchFactory;

        /// <summary>
        /// The login factory used to create login instances.
        /// </summary>
        private readonly ILoginFactory _loginFactory;

        /// <summary>
        /// The player dynamics service used to get player dynamics information.
        /// </summary>
        private readonly IPlayerDynamics _playerDynamics;

        /// <summary>
        /// The player recorder service used to record player activities and locations.
        /// </summary>
        private readonly IPlayerRecorder _playerRecorder;

        /// <summary>
        /// The cancellation token source used to cancel the location update task.
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// The current player.
        /// </summary>
        [ObservableProperty]
        private Player? _player;

        /// <summary>
        /// The current player's location.
        /// </summary>
        [ObservableProperty]
        private PlayerDynamicsLocation? _currentLocation;

        /// <summary>
        /// The current match.
        /// </summary>
        [ObservableProperty]
        private Match? _match;

        /// <summary>
        /// Indicates whether the object has been disposed.
        /// </summary>
        private bool _disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentGameViewModel"/> class.
        /// </summary>
        /// <param name="matchFactory">The match factory.</param>
        /// <param name="loginFactory">The login factory.</param>
        /// <param name="playerDynamics">The player dynamics service.</param>
        /// <param name="playerRecorder">The player recorder service.</param>
        /// <param name="loggedInUser">The logged in user.</param>
        public CurrentGameViewModel(IMatchFactory matchFactory, ILoginFactory loginFactory, IPlayerDynamics playerDynamics,
            IPlayerRecorder playerRecorder, LoggedInUser loggedInUser)
        {
            _matchFactory = matchFactory;
            _loginFactory = loginFactory;
            _playerDynamics = playerDynamics;
            _playerRecorder = playerRecorder;
            Player = loggedInUser as Player;
            _cancellationTokenSource = new CancellationTokenSource();
#if DEBUG
            AlwaysRecord = true;
#endif
        }

        /// <summary>
        /// Executes getting the data as the window loaded.
        /// </summary>
        public async Task ExecuteAsync()
        {
            Player ??= _loginFactory.CurrentUser as Player; // will never be null

            if (Player is not null)
            {
                // Get the player's next match from memory
                Match ??= await _matchFactory.GetPlyersNextMatch(playerId: Player.UserId).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// Starts the location update task.
        /// </summary>
        public void Start()
        {
            PayerIsInGame = true;

            if (Player is not null && Match is not null)
            {
                // Record the player's activity for the match
                _playerRecorder.RecordActivity(Player, Match, PlayerActivities.None);
            }

            // Start the task as a background task
            Task.Run(() => StartLocationUpdateTask(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Stops the location update task.
        /// </summary>
        public void Stop()
        {
            PayerIsInGame = false;
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to always record player activities.
        /// </summary>
        public bool AlwaysRecord { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is in the game.
        /// </summary>
        public bool PayerIsInGame
        {
            get => _playerDynamics.IsInGame;
            set
            {
                if (_playerDynamics.IsInGame != value)
                {
                    base.OnPropertyChanging(nameof(PayerIsInGame));

                    _playerDynamics.IsInGame = value;

                    base.OnPropertyChanged(nameof(PayerIsInGame));
                }
            }
        }

        /// <summary>
        /// Checks if the player has moved to a new location.
        /// </summary>
        /// <param name="location">The new location.</param>
        /// <returns><c>true</c> if the player has moved; otherwise, <c>false</c>.</returns>
        private bool Moved(PlayerDynamicsLocation location)
        {
            if (CurrentLocation is null)
            {
                return true;
            }

            return CurrentLocation.Lng != location.Lng || CurrentLocation.Lat != location.Lat;
        }

        /// <summary>
        /// Starts the location update task.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task representing the location update operation.</returns>
        private async Task StartLocationUpdateTask(CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                _playerDynamics.Match = Match;
                _playerDynamics.Member= Player;

                if (_playerDynamics.IsInGame && Player is not null && Match is not null)
                {
                    try
                    {
                        var startTime = IClock.Instance.Now;
                        var location = await _playerDynamics.GetPlayerDynamicsLocation().ConfigureAwait(false);
                        // Update the UI to show the player on the new location
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            CurrentLocation = location;
                        });
                        
                        // Record the player's location if it has changed
                        if (AlwaysRecord || Moved(location))
                        {
                            _playerRecorder.RecordLocation(Player, Match, location);
 
                        }

                        var endTime = IClock.Instance.Now;
                        var elapsedMilliseconds = (endTime - startTime).TotalMilliseconds;
                        var delay = TimeSpan.FromMilliseconds(1000 - elapsedMilliseconds);

                        // Simulate processing delay to make 1 sec, if it took too long then do not wait
                        if (delay.TotalMilliseconds > 0)
                        {
                            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        // Ignore as the user pressed stop
                    }
                    catch (Exception ex)
                    {
                        // Detect any crashes but continue the app
                        Walter.Inverse.GetLogger("CurrentGameViewModel")?.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="CurrentGameViewModel"/> and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        _cancellationTokenSource.Cancel();
                    }

                    _cancellationTokenSource.Dispose();
                }

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="CurrentGameViewModel"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
