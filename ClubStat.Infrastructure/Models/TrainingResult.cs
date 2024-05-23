namespace ClubStat.Infrastructure.Models;

/// <summary>
/// Dashboardplayer indicates the coaches opinion on the players training commitement
/// </summary>
/// <param name="Attitude"></param>
/// <param name="Commitment"></param>
public partial record TrainingResults(ScoreZeroToFive Attitude,ScoreZeroToFive Commitment);