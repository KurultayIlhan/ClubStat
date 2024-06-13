using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubStat.Infrastructure.Infrastructure
{
    public interface IOnlineDetector
    {
        bool IsOnline();
    }
}
