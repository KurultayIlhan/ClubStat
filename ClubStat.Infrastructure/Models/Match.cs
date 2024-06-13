using CommunityToolkit.Mvvm.ComponentModel;

using System.Data.SqlTypes;
using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{
    [JsonSerializable(typeof(Match))]
    [JsonSourceGenerationOptions(
                GenerationMode = JsonSourceGenerationMode.Metadata,                
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
        )]
    public partial class MatchJsonContext : JsonSerializerContext
    {
    }

    [JsonSerializable(typeof(List<Match>))]
    [JsonSourceGenerationOptions(
                GenerationMode = JsonSourceGenerationMode.Metadata,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
        )]
    public partial class MatchAgendaJsonContext : JsonSerializerContext { }

    public partial class Match : ObservableObject, IAsJson, IEquatable<Match?>
    {
       Club _homeTeam;
       Club _awayTeam;
       int _playerLeague;
       string _playerLeagueLevel;
       DateTime _matchDate;
       int _awayTeamGoals;
       int _homeTeamGoals;
       int _matchId;

        [JsonConstructor]
        public Match(Club homeTeam, Club awayTeam, int playerLeague, string playerLeagueLevel, DateTime matchDate
            , int awayTeamGoals, int homeTeamGoals, int matchId)
        {
            _homeTeam = homeTeam;
            _awayTeam = awayTeam;
            _playerLeagueLevel = playerLeagueLevel;
            _matchDate = matchDate;
            _awayTeamGoals = awayTeamGoals;
            _homeTeamGoals = homeTeamGoals;
            _matchId = matchId;
            _playerLeague = playerLeague;
        }

        public Club HomeTeam { get => _homeTeam; set =>SetProperty(ref _homeTeam , value); }
        public Club AwayTeam { get => _awayTeam; set =>SetProperty(ref _awayTeam , value); }
        public string PlayerLeagueLevel { get => _playerLeagueLevel; set => SetProperty(ref _playerLeagueLevel, value); }
        public int PlayerLeague { get => _playerLeague; set => SetProperty(ref _playerLeague, value); }

        public DateTime MatchDate 
        { 
            get => _matchDate;
            set => SetProperty(ref _matchDate, value);
                
            
        }
        
  
        
        public int AwayTeamGoals { get => _awayTeamGoals; set => SetProperty(ref _awayTeamGoals, value); }
        public int HomeTeamGoals { get => _homeTeamGoals; set => SetProperty(ref _homeTeamGoals, value); }
        public int MatchId { get => _matchId; set => SetProperty(ref _matchId, value); }

        public string AsJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(this, MatchJsonContext.Default.Match);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Match);
        }

        public bool Equals(Match? other)
        {
            return other is not null &&
                   EqualityComparer<Club>.Default.Equals(_homeTeam, other._homeTeam) &&
                   EqualityComparer<Club>.Default.Equals(_awayTeam, other._awayTeam) &&
                   _playerLeague == other._playerLeague &&
                   _playerLeagueLevel == other._playerLeagueLevel &&
                   _matchDate == other._matchDate &&
                   _awayTeamGoals == other._awayTeamGoals &&
                   _homeTeamGoals == other._homeTeamGoals &&
                   _matchId == other._matchId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_homeTeam, _awayTeam, _playerLeague, _playerLeagueLevel, _matchDate, _awayTeamGoals, _homeTeamGoals, _matchId);
        }

        public int GetReminderId()
        {
            return string.Concat(HomeTeam.ClubId.ToString(), AwayTeam.ClubId.ToString(), MatchDate.Ticks.ToString()).GetDeterministicHashCode();
        }

        public static bool operator ==(Match? left, Match? right)
        {
            return EqualityComparer<Match>.Default.Equals(left, right);
        }

        public static bool operator !=(Match? left, Match? right)
        {
            return !(left == right);
        }
    }
}
