using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClubStat.Infrastructure.Models
{
    [JsonSerializable(typeof(PlayerStatistics))]
    [JsonSourceGenerationOptions(
            GenerationMode = JsonSourceGenerationMode.Metadata,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
    )]
    public partial class PlayerStatisticsJsonContext : JsonSerializerContext { }

    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class PlayerStatistics
    {
        /// <summary>
        /// Gets or sets the goals.
        /// </summary>
        /// <remarks>records the activities of the number of <see cref="PlayerActivities.Goal"/>recorded in activities</remarks>
        /// <value>The goals.</value>
        public int Goals { get; set; }
        /// <summary>
        /// Gets or sets the assists.
        /// </summary>
        /// <remarks>records the activities of the number of <see cref="PlayerActivities.Assist"/>recorded in activities</remarks>
        /// <value>The assists.</value>
        public int Assists { get; set; }
        /// <summary>
        /// Gets or sets the yellow.
        /// </summary>
       /// <remarks>records the activities of the number of <see cref="PlayerActivities.YellowCard"/>recorded in activities</remarks>
        /// <value>The yellow.</value>
        public int Yellow { get; set; }
        /// <summary>
        /// Gets or sets the red.
        /// </summary>
        /// <value>The red.</value>
        /// <remarks>records the activities of the number of <see cref="PlayerActivities.RedCard"/>recorded in activities</remarks>
        public int Red { get; set; }

        /// <summary>
        /// Gets or sets the number of matches where the player had recorded activities.
        /// </summary>
        /// <value>The matches.</value>
        public int Matches { get; set; }

        /// <summary>
        /// Gets or sets from date in UTC for the first recorded activity.
        /// </summary>
        /// <value>Date in UTC format</value>
        public DateTime FromUtc { get; set; }

        /// <summary>
        /// Gets or sets from date in UTC for the Last recorded activity.
        /// </summary>
        /// <value>The till UTC.</value>
        public DateTime TillUtc { get; set; }

        private string GetDebuggerDisplay()
        {
            return $"Between {FromUtc.ToShortDateString()} till {TillUtc.ToShortDateString()}";
        }
    }
}
