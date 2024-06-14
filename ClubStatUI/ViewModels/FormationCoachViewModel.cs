// ***********************************************************************
// Assembly         : 
// Author           : Ilhan Kurultay
// Created          : Sat 01-Jun-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Wed 05-Jun-2024
// ***********************************************************************
// <copyright file="FormationCoachViewModel.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary>
// deals with saving player locations during a match as well as when planning a game.
// </summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ClubStatUI.ViewModels
{
    public partial class FormationCoachViewModel : ObservableObject, ILoadAsync, IPlayerImageLoader
    {
        private readonly IMatchFactory _matchFactory;
        private readonly IClubFactory _clubFactory;
        private readonly IProfilePictureFactory _pictureFactory;
        private readonly IPlayerRecorder _recorder;
        private readonly IPlayerMentainance _playerMentainance;
        private readonly IMessageDialog _messageDialog;
        private Timer _locationTimer;

        //public ICommand DropCommand { get; }
        public ICommand FormationChangedCommand { get; }

        [ObservableProperty]
        private ClubMember _user;

        [ObservableProperty]
        private ObservableCollection<PlayerMovement> _playerMovements;

        [ObservableProperty]
        Player? _SP;
        [ObservableProperty]
        Player? _LM;
        [ObservableProperty]
        Player? _CAM;
        [ObservableProperty]
        Player? _RM;
        [ObservableProperty]
        Player? _CM;
        [ObservableProperty]
        Player? _CML;
        [ObservableProperty]
        Player? _CMR;
        [ObservableProperty]
        Player? _LB;
        [ObservableProperty]
        Player? _RB;
        [ObservableProperty]
        Player? _CBR;
        [ObservableProperty]
        Player? _CBL;
        [ObservableProperty]
        Player? _GK;
        [ObservableProperty]
        Player? _BankOne;
        [ObservableProperty]
        private Match? _match;

        [ObservableProperty]
        private ObservableCollection<Player> _players;

        [ObservableProperty]
        private ObservableCollection<Player> _playersWithoutPosition;

        [ObservableProperty]
        private List<PlayerMovement>? _teamsLastKnownLocation;

        [ObservableProperty]
        private PlayerActivities _selectedActivity = PlayerActivities.None;

        /// <summary>
        /// The selected player is used to update stats etc
        /// </summary>
        private Player? _selectedPlayer;

        [ObservableProperty]
        private PlayerDynamicsLocation? _selectedPlayerPosition;

        [ObservableProperty]
        private PlayerGameMotivation? _playerGameMotivation;

        [ObservableProperty]
        private bool _canUpdatePlayer;

        public FormationCoachViewModel(
            ILoginFactory loginFactory,
            IMatchFactory matchFactory,
            LoggedInUser coach,
            IClubFactory clubFactory,
            IProfilePictureFactory pictureFactory,
            IPlayerRecorder recorder,
            IPlayerMentainance playerMentainance,
            IMessageDialog messageDialog
            )
        {
            _matchFactory = matchFactory;
            _clubFactory = clubFactory;
            _pictureFactory = pictureFactory;
            _recorder = recorder;
            _playerMentainance = playerMentainance;
            _messageDialog = messageDialog;
            _user = (ClubMember)coach;
            _playersWithoutPosition = new ObservableCollection<Player>();
            _players = new ObservableCollection<Player>();
            _playerMovements = new ObservableCollection<PlayerMovement>();

            _locationTimer = new Timer(LoadPlayersLastKnownLocation, _selectedPlayer, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            // DropCommand = new Command<DropEventArgs>(OnDrop);
            //DropOccurred = new Command<DropEventArgs>();
            FormationChangedCommand = new Command<string>(UpdateFormationGrid);



        }

        volatile bool _bussyLoadingLocation;
        private async void LoadPlayersLastKnownLocation(object? state)
        {
            if (_bussyLoadingLocation) return;
            if (Match is null || SelectedPlayer is null) return;

            _bussyLoadingLocation = true;

            _locationTimer.Change(Timeout.Infinite, Timeout.Infinite);
            var recorded = DateTime.Now;
            try
            {

                var entry = await _recorder.GetPlayersLastKnownLocation(SelectedPlayer, Match)
                                           .ConfigureAwait(false);
                if (entry != null)
                {
                    var pos = new PlayerDynamicsLocation(entry.Latitude
                                                        , entry.Longitude
                                                        , entry.RecordedUtc.ToLocalTime()
                                                        , entry.MatchId
                                                        , SelectedPlayer.UserId
                                                        );
                    if (MainThread.IsMainThread)
                    {
                        SelectedPlayerPosition = pos;
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            SelectedPlayerPosition = pos;
                        });
                    }
                }
            }
            finally
            {
                _bussyLoadingLocation = false;
                //start the timer again, if all is ok we had time till the second passed
                var elapsedMilliseconds = (DateTime.Now - recorded).TotalMilliseconds;
                var wait = 1000 - elapsedMilliseconds;
                if (wait > 0)
                {
                    _locationTimer.Change(TimeSpan.FromMilliseconds(wait), TimeSpan.FromSeconds(1));
                }
                else
                {
                    _locationTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
                }
            }
        }



        /// <summary>
        /// Gets the teams last known location.
        /// </summary>
        /// <remarks>
        /// https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/relaycommand
        /// </remarks>
        [RelayCommand]
        private async Task GetTeamsLastKnownLocation(int? matchId)
        {

            if (!matchId.HasValue) return;

            var team = await _recorder.GetTeamsLastKnownLocation(User.ClubId, matchId.Value).ConfigureAwait(true);
            if (team is not null)
            {
                TeamsLastKnownLocation = team;
            }

        }

        public Player? SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                SetProperty(ref _selectedPlayer, value);
            }
        }


        [RelayCommand()]
        private async Task UpdatePlayerStatistics()
        {
            if (PlayerGameMotivation is not null)
            {
                await _playerMentainance.SavePlayerMotivation(PlayerGameMotivation);
            }
        }

        [RelayCommand()]
        private async Task UpdatePlayerActivity(PlayerActivities playerActivities)
        {
            if (SelectedPlayer is null || Match is null || playerActivities == PlayerActivities.None) return;

            var saved = await _recorder.RecordActivity(SelectedPlayer, Match, activity: playerActivities)
                                      .ConfigureAwait(false);
            if (!saved)
            {
                //offline?
                _messageDialog.ShowMessage("Could not save the status, are you offline?");
            }

            SelectedActivity = PlayerActivities.None;
        }
        [RelayCommand()]
        public async Task UpdateSelectPlayer(Player player)
        {
            await GetSelectedPlayerGameMotivation(player).ConfigureAwait(true);
            _selectedPlayer = player;

            base.OnPropertyChanging(nameof(SelectedPlayer));
        }

        [RelayCommand()]
        private async Task GetSelectedPlayerGameMotivation(Player player)
        {
            if (SelectedPlayer == player) return;
            if (Match is null) return;
            var samePlayer = PlayerGameMotivation is not null && PlayerGameMotivation.PlayerId == player.UserId;
            var sameMatch = PlayerGameMotivation is not null && PlayerGameMotivation.MatchId == Match.MatchId;
            if (samePlayer && sameMatch) return;
            //do not get it if it's already the current player on the current match
            var model = new PlayerGameMotivation(player.UserId, Match.MatchId, 0, 0);
            var answer = await _playerMentainance.GetGameMotivationAsync(model)
                                                  .ConfigureAwait(true);
            this.PlayerGameMotivation = answer;
            SelectedPlayer = player;
            //if the selected user is still the one we requested the data for
        }


        public async Task MovePlayerOffField(string? automationId, Player? newPlayer)
        {
            if (string.IsNullOrEmpty(automationId) || PlayersWithoutPosition == null) return;

            var oldPlayer = automationId switch
            {
                "SP" => SP,
                "LM" => LM,
                "RM" => RM,
                "CAM" => CAM,
                "CML" => CML,
                "CMR" => CMR,
                "LB" => LB,
                "RB" => RB,
                "CBR" => CBR,
                "CBL" => CBL,
                "GK" => GK,
                "BankOne" => BankOne,
                _ => null
            };
            //update the new player
            if (Enum.TryParse<MatchPosition>(automationId, out var position) && newPlayer is not null && Match is not null)
            {
                await _matchFactory.UpdateMatchPositionAsync(position, newPlayer.UserId, Match.MatchId, Match.MatchDate)
                                    .ConfigureAwait(true);
                await GetSelectedPlayerGameMotivation(newPlayer)
                                    .ConfigureAwait(true);
                SelectedPlayer = newPlayer;
                PlayersWithoutPosition.Remove(newPlayer);
                switch (automationId)
                {
                    case "SP":
                        SP = newPlayer;
                        break;
                    case "LM":
                        LM = newPlayer;
                        break;
                    case "RM":
                        RM = newPlayer;
                        break;
                    case "CAM":
                        CAM = newPlayer;
                        break;
                    case "CML":
                        CML = newPlayer;
                        break;
                    case "CMR":
                        CMR = newPlayer;
                        break;
                    case "LB":
                        LB = newPlayer;
                        break;
                    case "RB":
                        RB = newPlayer;
                        break;
                    case "CBR":
                        CBR = newPlayer;
                        break;
                    case "CBL":
                        CBL = newPlayer;
                        break;
                    case "GK":
                        GK = newPlayer;
                        break;
                    case "BankOne":
                        BankOne = newPlayer;
                        break;
                }
            }
            //remove the old player
            if (oldPlayer is not null && Match is not null)
            {
                PlayersWithoutPosition.Add(oldPlayer);

                await _matchFactory.UpdateMatchPositionAsync(MatchPosition.None, oldPlayer.UserId, Match.MatchId, Match.MatchDate);


            }
        }


        public async Task ExecuteAsync()
        {
            List<Player>? _players = null;
            if (User is Coach coach)
            {
                Match = await _matchFactory.GetCoachsNextMatch(User.UserId);
                _players = await _clubFactory.GetTeamPlayers(coach.ClubId, coach.PlayersLeague, coach.PlayersLeagueLevel);
            }
            else if (User is Player player)
            {
                Match = await _matchFactory.GetPlyersNextMatch(User.UserId);
                _players = await _clubFactory.GetTeamPlayers(player.ClubId, player.PlayersLeague, player.PlayersLeagueLevel);
                await UpdateSelectPlayer(player);
            }

            //user can't update own motivation
            CanUpdatePlayer = User.UserType != UserType.Player;

            if (_players != null)
            {
                if (_players.Count > 0 && User.UserType == UserType.Coach)
                {
                    await UpdateSelectPlayer(_players[0]);
                }
                Players = new ObservableCollection<Player>(_players);

                if (Match != null)
                {
                    var playersOnPositions = await _matchFactory.GetMatchPositionAsync(Match.MatchId).ConfigureAwait(true);
                    PlayersWithoutPosition = new ObservableCollection<Player>(_players.Where(player => !playersOnPositions.Any(position => position.PlayerId == player.UserId)));

                    foreach (var entry in playersOnPositions)
                    {
                        var player = Players.FirstOrDefault(f => f.UserId.Equals(entry.PlayerId));
                        switch (entry.Position)
                        {
                            case MatchPosition.SP: SP = player; break;
                            case MatchPosition.LM: LM = player; break;
                            case MatchPosition.RM: RM = player; break;
                            case MatchPosition.CAM: CAM = player; break;
                            case MatchPosition.CML: CML = player; break;
                            case MatchPosition.CMR: CMR = player; break;
                            case MatchPosition.LB: LB = player; break;
                            case MatchPosition.RB: RB = player; break;
                            case MatchPosition.CBR: CBR = player; break;
                            case MatchPosition.CBL: CBL = player; break;
                            case MatchPosition.GK: GK = player; break;
                            case MatchPosition.BankOne: BankOne = player; break;
                        }
                    }
                }
                else
                {
                    PlayersWithoutPosition = new ObservableCollection<Player>(_players);
                }


            }
        }

        public Task<byte[]> GetPlayerImageAsync(Guid userId)
        {
            return _pictureFactory.GetProfilePictureForUserAsync(userId);
        }

        public async Task OnDragStarting(DragStartingEventArgs e)
        {
            if (e is null) return;


            if (e.Data.Properties["Player"] is Player player)
            {
                var bytes = await GetPlayerImageAsync(player.UserId).ConfigureAwait(true);
                if (bytes.Length > 0)
                {
                    e.Data.Properties.Add("PlayerImage", bytes);
                    var imageSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                    e.Data.Image = imageSource;
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void OnDrop(DropEventArgs e)
        {
            if (e is null) return;
            if (e.Data.Properties.ContainsKey("Player") && e.Data.Properties["Player"] is Player player)
            {


            }
        }

        private void UpdateFormationGrid(string? formation)
        {
            if (string.IsNullOrEmpty(formation)) return;

            switch (formation)
            {
                case "4-2-3-1":
                    // Update grid positions for each player
                    break;
                case "4-3-3":
                    // Update grid positions for each player
                    break;
                case "4-4-2":
                    // Update grid positions for each player
                    break;
            }
        }
    }


}