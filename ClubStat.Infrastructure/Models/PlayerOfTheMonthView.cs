namespace ClubStat.Infrastructure.Models;

/// <summary>
/// PLayer Dashboard shows the player of the month for the club 
/// </summary>
/// <remarks>Dashboard will only show for logged in users team</remarks>
/// <param name="Team">Team where the player plays for</param>
/// <param name="Player">Awarded player of the month</param>
public record PlayerOfTheMonthView(Team Team, Player Player);