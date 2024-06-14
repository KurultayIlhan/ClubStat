// ***********************************************************************
// Assembly         : ClubStat.RestServer
// Author           : Ilhan
// Created          : Thu 16-May-2024
//
// Last Modified By : Walter Verhoeven
// Last Modified On : Wed 12-Jun-2024
// ***********************************************************************
// <copyright file="MatchController.cs" company="Ilhan">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Models;
using ClubStat.RestServer.Infrastructure;

using Microsoft.AspNetCore.Mvc;

namespace ClubStat.RestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly DbHelper _db;
        private readonly ILogger<MatchController> _logger;

        public MatchController(DbHelper db, ILogger<MatchController> logger)
        {
            _db = db;
            _logger = logger;
        }
        [HttpGet("/api/match/GetPreviousMatchesForPlayer/{playerId}")]
        public async Task<IActionResult> GetPreviousMatchesForPlayer(Guid playerId)
        {
            _logger.LogDebug("Call GetPreviousMatchesForPlayer for Player {playerId}", playerId);
            try
            {
                Match? answer = await _db.GetPreviousMatchesForPlayerAsync(playerId).ConfigureAwait(false);
                if (answer is null)
                {
                    return NotFound("Player is not known or player did not play in any matches");
                }
                return Ok(answer);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("/api/match/GetNextMatchForCoach/{coachId}")]
        public async Task<IActionResult> GetNextMatchForCoach(Guid coachId)
        {
            _logger.LogDebug("Call GetNextMatchForCoach for coach {coachId}", coachId);
            try
            {
                Match? answer = await _db.GetNextMatchForCoachAsync(coachId).ConfigureAwait(false);
                if (answer is null)
                {
                    return NotFound("Player is not known or player did not play in any matches");
                }
                return Ok(answer);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("/api/match/GetPreviouseMatchForCoach/{coachId}")]
        public async Task<IActionResult> GetPreviouseMatchForCoach(Guid coachId)
        {
            _logger.LogDebug("Call GetPreviouseMatchForCoach coach {coachId}", coachId);

            try
            {
                Match? answer = await _db.GetPreviouseMatchForCoachAsync(coachId).ConfigureAwait(false);
                if (answer is null)
                {
                    return NotFound("Player is not known or player did not play in any matches");
                }
                return Ok(answer);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("/api/match/GetPlayerAgenda/{playerId}")]
        public async Task<IActionResult> GetPlayerAgenda(Guid playerId)
        {
            _logger.LogDebug("Call GetPlayerAgenda for player {playerId}", playerId);
            try
            {
                List<Match> answer = await _db.GetPlayerAgendaAsync(playerId).ConfigureAwait(false);
                _logger.LogDebug("Database returned {count} entries for {playerId}", answer.Count, playerId);

                return Ok(answer);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("/api/match/PlayerPositions/{matchId}")]
        public async Task<IActionResult> GetPlayerMatchPositions(int matchId)
        {
            _logger.LogDebug("Call GetPlayerMatchPositions for match {matchId}", matchId);
            try
            {
                List<PlayerMatchPosition> answer = await _db.GetPlayerMatchPositionsAsync(matchId).ConfigureAwait(false);
                if (answer.Count == 0) return NotFound("No such match exists");

                _logger.LogDebug("Database returned {count} entries for {matchId}", answer.Count, matchId);
                return Ok(answer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting match positions for {matchId}", matchId);
                return BadRequest(ex);
            }
        }

        [HttpGet("/api/match/LastKnownLocation/{matchId}/{playerId}")]
        public async Task<IActionResult> GetPlayersLastKnownLocation(int matchId, Guid playerId)
        {
            _logger.LogDebug("Call GetPlayersLastKnownLocation for match {matchId} and player {playerId}", matchId, playerId);
            try
            {
                var answer = await _db.GetPlayersLastKnownLocation(matchId, playerId).ConfigureAwait(false);
                if (answer is null) return NotFound("No such player found in the provided match.");

                _logger.LogDebug("Successfully retrieved last known location for match {matchId} and player {playerId}", matchId, playerId);
                return Ok(answer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting match positions for {matchId}", matchId);
                return BadRequest(ex);
            }
        }

        [HttpPost("/api/match/PlayerPositions")]
        public async Task<IActionResult> SetPlayerMatchPositions([FromBody] PlayerMatchPosition model)
        {
            _logger.LogDebug("POST SetPlayerMatchPositions for match {matchId} and player {playerId}", model.MatchId, model.PlayerId);
            try
            {
               var sucess= await _db.SetPlayerMatchPositionsAsync(
                    position: model.Position,
                    matchId: model.MatchId,
                    playerId: model.PlayerId,
                    onField: model.OnFieldUtc,
                    offField: model.OffFieldUtc
                ).ConfigureAwait(false);

                if(sucess)
                    _logger.LogDebug("Successfully set player positions for match {matchId} and player {playerId}", model.MatchId, model.PlayerId);
                else
                    _logger.LogInformation("No data changes detected while set player positions for match {matchId} and player {playerId}", model.MatchId, model.PlayerId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting match positions for match {matchId} and player {playerId}", model.MatchId, model.PlayerId);
                return BadRequest(ex);
            }
        }
    }
}
