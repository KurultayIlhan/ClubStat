using ClubStat.Infrastructure.Models;

using Microsoft.Extensions.Caching.Memory;

namespace ClubStat.RestServer.Infrastructure
{
    public interface ISessionService
    {
        Session CreateSession(string token);
        Session? ValidateSession(string token);
        void InvalidateSession(string token);
    }

    class SessionService : ISessionService
    {
        private readonly IMemoryCache _memory;

        public SessionService(IMemoryCache memory)
        {
            _memory = memory;
        }
        public Session CreateSession(string token)
        {
            var session=new Session() { Token = token, Expiry = DateTime.Now.AddMinutes(20) };
            _memory.Set(token,session, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(30) });
            return session;

        }

        public void InvalidateSession(string token)
        {
            _memory.Remove(token);
        }

        public Session? ValidateSession(string token)
        {
            if (_memory.TryGetValue<Session>(token, out var session))
            {
                if (session is not null)
                {
                    session.Expiry = DateTime.Now.AddMinutes(20);
                }                
            }

            return session;
        }
    }
}
