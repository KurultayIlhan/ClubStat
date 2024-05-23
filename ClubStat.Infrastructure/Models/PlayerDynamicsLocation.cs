using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubStat.Infrastructure.Models
{
    public class PlayerDynamicsLocation
    {
        public PlayerDynamicsLocation(decimal lat, decimal lng, DateTime recorded )
        {
            Lat = lat;
            Lng = lng;
            Recorded = recorded;
        }

        public decimal Lat { get; }
        public decimal Lng { get; }
        public DateTime Recorded { get; }
    }
}
