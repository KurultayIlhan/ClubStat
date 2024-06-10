using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

namespace ClubStatUI.ViewModels
{
    [QueryProperty(nameof(Player), nameof(Player))]
    [QueryProperty(nameof(Match), nameof(Match))]
    public partial class CurrentGameViewModel : ObservableObject, ILoadAsync, IDisposable
    {
        private readonly IMatchFactory _matchFactory;
        private readonly ILoginFactory _loginFactory;
        private readonly IPlayerDynamics _playerDynamics;
        private readonly IPlayerRecorder _playerRecorder;
        private readonly CancellationTokenSource _cancellationTokenSource;

        [ObservableProperty]
        private Player? _player;
        [ObservableProperty]
        private PlayerDynamicsLocation? _currentLocation;

        [ObservableProperty]
        private Match? _match;
        private bool _disposedValue;

        public CurrentGameViewModel(IMatchFactory matchFactory, ILoginFactory loginFactory, IPlayerDynamics playerDynamics, IPlayerRecorder playerRecorder, LoggedInUser loggedInUser)
        {
            _matchFactory = matchFactory;
            _loginFactory = loginFactory;
            _playerDynamics = playerDynamics;
            _playerRecorder = playerRecorder;
            Player = loggedInUser as Player;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task ExecuteAsync()
        {
            Player ??= _loginFactory.CurrentUser as Player;

            if (Player is not null)
            {
                Match ??= await _matchFactory.GetPlyersNextMatch(playerId: Player.UserId).ConfigureAwait(true);
            }
        }
        public void Start()
        {
            PayerIsInGame = true;
            StartLocationUpdateTask(_cancellationTokenSource.Token);

        }

        public void Stop()
        {
            PayerIsInGame = false;
            _cancellationTokenSource.Cancel();
        }


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

        private bool Moved(PlayerDynamicsLocation location)
        {
            if (CurrentLocation is null) { return true; }

            return CurrentLocation.Lng != location.Lng || CurrentLocation.Lat != location.Lat;
        }


        private void StartLocationUpdateTask(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_playerDynamics.IsInGame && Player is not null && Match is not null)
                    {
                        var location = await _playerDynamics.GetPlayerDynamicsLocation().ConfigureAwait(false);

                        if (Moved(location))
                        {
                            _playerRecorder.RecordLocation(Player, Match, location);

                            //update the UI show the player on the new location
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                CurrentLocation = location;
                            });
                        }
                        // Simulate processing delay
                        await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                    }
                }
            }, cancellationToken);
        }

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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CurrentGameViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
