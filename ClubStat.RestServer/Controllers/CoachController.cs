using ClubStat.Infrastructure.Models;
using ClubStat.RestServer.Infrastructure;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClubStat.RestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoachController : ControllerBase
    {
        private readonly DbHelper _db;
        private readonly ILogger _logger;
        public CoachController(DbHelper db, ILogger<CoachController> logger )
        {
            _db = db;
            this._logger = logger;
        }

        [HttpGet("/api/Coach/{id:guid}")]
        public async Task<IActionResult> GetCoach(Guid id)
        {
            _logger.LogInformation("Get GetCoach for team id {id}",id);
            if (Guid.Equals(id, Guid.Empty))
            { 
                return NotFound("The provided parameter is not a guid we can work with");
            }
            try
            {
                var answer = await _db.GetCoachAsync(id).ConfigureAwait(false);
                if (answer is not null)
                {
                    return Ok(answer);
                }
                return BadRequest("Answer was not returned from the database");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }

        }
    }

}