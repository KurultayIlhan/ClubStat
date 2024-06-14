using ClubStat.Infrastructure.Models;
using ClubStat.RestServer.Infrastructure;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClubStat.RestServer.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly DbHelper _db;
        private readonly ILogger<ClubController> _logger;

        public ClubController(DbHelper db, ILogger<ClubController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("/api/club/GetTeam/{clubId:int}/{age:int}/{playersLeagueLevel}")]
        public async Task<IActionResult> GetTeam(int clubId, int age, string playersLeagueLevel)
        {
            _logger.LogDebug("GetTeam called with {clubId},{age},{league}", clubId, age, playersLeagueLevel);

            try
            {
                List<Player> team = await _db.GetTeamAsync(clubId, age, playersLeagueLevel[0]).ConfigureAwait(false);
                if (team.Count == 0)
                {
                    return NotFound("No team registered with the club on that age and league");
                }
                return Ok(team);
            }
            catch (Exception ex)
            {                
                _logger.LogException(ex);
                return BadRequest(ex);
            }
            
        }

        [HttpGet("/api/club/LastKnownLocation/{clubId}/{matchId}")]
        public async Task<IActionResult> GetTeamPosition(int clubId, int matchId)
        {
            _logger.LogDebug("GetTeamPosition with {clubId}, {matchId}", clubId, matchId);
            try
            {
                List<PlayerMovement> positions = await _db.GetTeamLocationAsync(clubId, matchId).ConfigureAwait(false);
                if (positions.Count == 0)
                {
                    return NotFound("No team registered with the club on that age and league");
                }
                return Ok(positions);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }
    }
}
