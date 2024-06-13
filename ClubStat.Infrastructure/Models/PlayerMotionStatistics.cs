using System.Text.Json.Serialization;

namespace ClubStat.Infrastructure.Models
{
    [JsonSerializable(typeof(PlayerMotionStatistics))]
    [JsonSourceGenerationOptions(
        GenerationMode = JsonSourceGenerationMode.Metadata,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
)]
    public partial class PlayerMotionStatisticsJsonContext : JsonSerializerContext
    {
    }

    /// <summary>
    /// Class PlayerMotionStatistics.
    /// </summary>
    public class PlayerMotionStatistics : IAsJson
    {
        /// <summary>
        /// Gets or sets the match identifier.
        /// </summary>
        /// <value>The match identifier.</value>
        public int MatchId { get; set; }
        /// <summary>
        /// Gets or sets the player identifier.
        /// </summary>
        /// <value>The player identifier.</value>
        public Guid PlayerId { get; set; }
        /// <summary>
        /// Gets or sets the sprints (top speed).
        /// </summary>
        /// <value>The sprints.</value>
        public double Sprints { get; set; }
        /// <summary>
        /// Gets or sets the median speed.
        /// </summary>
        /// <value>The median speed.</value>
        public double MedianSpeed { get; set; }
        /// <summary>
        /// Gets or sets the top speed.
        /// </summary>
        /// <value>The top speed.</value>
        public double TopSpeed { get; set; }
        /// <summary>
        /// Gets or sets the average speed.
        /// </summary>
        /// <value>The average speed.</value>
        public double AverageSpeed { get; set; }
        /// <summary>
        /// Gets a value indicating whether played in match.
        /// </summary>
        /// <remarks>Did not move== Did not play</remarks>
        /// <value><c>true</c> if [played in match]; otherwise, <c>false</c>.</value>
        public bool PlayedInMatch=> MedianSpeed > 0 ;

        public string AsJson()
        {
            //PlayerMotionStatisticsJsonContext
            return System.Text.Json.JsonSerializer.Serialize(this, PlayerMotionStatisticsJsonContext.Default.PlayerMotionStatistics);
        }
    }
}
