using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubStat.Infrastructure
{
    public interface IPlayerImageLoader
    {
        Task<byte[]> GetPlayerImageAsync(Guid userId);
    }
}
