using Microsoft.Extensions.Caching.Memory;

namespace ClubStat.Infrastructure.Factories
{

    public interface IMatchFactory
    {
        Task<Match?> GetPlyersNextMatch(Guid playerId);
        Task<Match?> GetPlayersLastMatch(Guid playerId);
        Task<Match?> GetCoachsNextMatch(Guid coachId);
        Task<Match?> GetPreviouseMatchForCoach(Guid coachId);
        Task<List<Match>> GetPlayersAgendaAsync(Guid playerId);
        Task<List<PlayerMatchPosition>> GetMatchPositionAsync(int matchId);
        Task UpdateMatchPositionAsync(MatchPosition position, Guid? userId, int matchId, DateTime date);
    }
    internal class MatchFactory : ApiBasedFactory, IMatchFactory
    {
        private readonly IMemoryCache _memoryCache;

        public MatchFactory(IConfiguration configuration, IHttpClientFactory clientFactory, IMemoryCache memoryCache)
            : base(configuration, clientFactory)
        {
            _memoryCache = memoryCache;
        }

        public async Task<Match?> GetPlyersNextMatch(Guid playerId)
        {
            var key = $"GetPlyersNextMatch-{playerId}";
            if (_memoryCache.TryGetValue<Match>(key, out var cashed))
                return cashed;

            var match = await GetAsync<Match>($"{MagicStrings.PlayerNextMatch}/{playerId}", MatchJsonContext.Default.Match).ConfigureAwait(false);
            if (match is not null)
            {
                _memoryCache.Set(key, match, absoluteExpiration: new DateTimeOffset(match.MatchDate.ToLocalTime()));
            }
            return match;
        }

        public async Task<Match?> GetPlayersLastMatch(Guid playerId)
        {
            var key = $"GetPlayersLastMatch-{playerId}";
            if (_memoryCache.TryGetValue<Match>(key, out var cashed))
                return cashed;
            var url = $"api/match/GetPreviousMatchesForPlayer/{playerId}";
            var match = await GetAsync<Match>(url, MatchJsonContext.Default.Match).ConfigureAwait(false);
            if (match is not null)
            {
                _memoryCache.Set(key, match, absoluteExpiration: new DateTimeOffset(DateTime.Now.AddMinutes(10)));
            }
            return match;
        }

        public async Task<Match?> GetCoachsNextMatch(Guid coachId)
        {
            var key = $"GetCoachsNextMatch-{coachId}";
            if (_memoryCache.TryGetValue<Match>(key, out var cashed))
                return cashed;

            var url = $"api/match/GetNextMatchForCoach/{coachId}";
            var match = await GetAsync<Match>(url, MatchJsonContext.Default.Match).ConfigureAwait(false);

            if (match is not null)
            {
                _memoryCache.Set(key, match, absoluteExpiration: new DateTimeOffset(DateTime.Now.AddMinutes(10)));
            }

            return match;
        }
        public async Task<Match?> GetPreviouseMatchForCoach(Guid coachId)
        {
            var url = $"api/match/GetPreviouseMatchForCoach/{coachId}";
            if (_memoryCache.TryGetValue<Match>(url, out var cashed))
                return cashed;
            var match = await GetAsync<Match>(url, MatchJsonContext.Default.Match).ConfigureAwait(false);
            if (match is not null)
            {
                _memoryCache.Set(url, match, absoluteExpiration: new DateTimeOffset(DateTime.Now.AddMinutes(10)));
            }

            return match;
        }

        public async Task<List<Match>> GetPlayersAgendaAsync(Guid playerId)
        {
            var answer = new List<Match>();
            var url = $"api/match/GetPlayerAgenda/{playerId}";
            if (_memoryCache.TryGetValue<List<Match>>(url, out var cashed) && cashed is not null && cashed.Count > 0)
                return cashed;

            var match = await GetAsync<List<Match>>(url, MatchAgendaJsonContext.Default.ListMatch).ConfigureAwait(false);
            if (match is not null)
            {
                foreach (var item in match)
                {
                    answer.Add(item);
                }
            }

            if (match is not null)
            {
                _memoryCache.Set(url, match, absoluteExpiration: new DateTimeOffset(DateTime.Now.AddMinutes(10)));
            }
            return answer;
        }

        static string GetMatchPositionCashingKey(int matchId) => $"api/match/PlayerPositions/{matchId}";
        public async Task<List<PlayerMatchPosition>> GetMatchPositionAsync(int matchId)
        {
            var url = $"api/match/PlayerPositions/{matchId}";
            if (_memoryCache.TryGetValue<List<PlayerMatchPosition>>(url, out var cashed)
                && cashed is not null && cashed.Count > 0)
                return cashed;

            var answer = await GetAsync<List<PlayerMatchPosition>>(url, PlayerMatchPositionListJsonContext.Default.ListPlayerMatchPosition).ConfigureAwait(false);
            if (answer is not null && answer.Count > 0)
            {
                _memoryCache.Set(url, answer);
            }
            return answer ?? new List<PlayerMatchPosition>();
        }

        public async Task UpdateMatchPositionAsync(MatchPosition position, Guid? userId, int matchId, DateTime date)
        {
            var key = GetMatchPositionCashingKey(matchId);
            if (_memoryCache.TryGetValue(key, out _))
            {
                _memoryCache.Remove(key);
            }
            var pos = new PlayerMatchPosition()
            {
                MatchId = matchId
                        ,
                PlayerId = userId ?? Guid.Empty
                        ,
                Position = position
                        ,
                OnFieldUtc = date
                        ,
                OffFieldUtc = date > DateTime.UtcNow ? date : (DateTime?)null
            };

            await base.WriteDataAsync(MagicStrings.RecordPlayerMatchPosition, pos).ConfigureAwait(false);

        }
    }
}
