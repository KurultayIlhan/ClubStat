using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;


namespace ClubStatUI.ViewModels
{

    public partial class ProfilePlayerViewModel : ObservableObject, ILoadAsync
    {
        private readonly IPlayerRecorder _playerRecorder;
        private readonly IMatchFactory _matchFactory;
        [ObservableProperty]
        private PlayerStatistics _statistics = new();

        [ObservableProperty]
        Player? _player;

        [ObservableProperty]
        Match? _lastMatch;

        [ObservableProperty]
        List<PlayerActivityRow>? _playerActivityRows;
        public ProfilePlayerViewModel(IPlayerRecorder playerRecorder, ILoginFactory loginFactory, IMatchFactory matchFactory)
        {
            _playerRecorder = playerRecorder;
            _matchFactory = matchFactory;
            if (loginFactory.CurrentUser is Player player)
            {
                Player = player;
            }

        }
        public async Task ExecuteAsync()
        {
            if (Player is not null)
            {
                Statistics = await _playerRecorder.GetPlayerStatisticsAsync(Player).ConfigureAwait(true);
                LastMatch = await _matchFactory.GetPlayersLastMatch(Player.UserId).ConfigureAwait(true);
                if (LastMatch is not null)
                {
                    var stats = await _playerRecorder.GetRecordActivity(Player, LastMatch).ConfigureAwait(true);
                    if (stats.Count > 0)
                    {
                        PlayerActivityRows = stats;
                    }
                }
            }
        }
    }
}
