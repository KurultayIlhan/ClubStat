// ***********************************************************************
// Assembly         : ClubStat.RestServer
// Author           : Ilhan
// Created          : Sun 12-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Sun 12-May-2024
// ***********************************************************************
// <copyright file="PlayerController.cs" company="ClubStat.RestServer">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary>
// Task 9: Player API Andponz
// </summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Models;
using ClubStat.RestServer.Infrastructure;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace ClubStat.RestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly DbHelper _db;

        public PlayerController(DbHelper db, ILogger<PlayerController> logger)
        {
            this._db = db;
            this._logger = logger;
        }



        [HttpGet("/api/Player/ProfileImage/{id}")]
        public async Task<IActionResult> GetProfileImage(Guid id)
        {
            _logger.LogDebug("GetProfileImage for {id}", id);
            try
            {
                byte[] bytes = await _db.GetProfileImageForAsync(userId: id).ConfigureAwait(false);
                if (bytes.Length != 0)
                {
                    return File(bytes, "image/png");
                }
                var assembly = this.GetType().Assembly;
                using var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().First(name => name.EndsWith("playerNotFound.png")));
                if (stream is not null)
                {
                    var reader = new MemoryStream();
                    await stream.CopyToAsync(reader).ConfigureAwait(false);
                    bytes = reader.ToArray();

                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Found;
                    return File(bytes, "image/png", "playerNotFound.png", false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
            return NotFound();
        }
        [HttpGet("/api/player/Statistics/{userId}/{afterDate}")]
        public async Task<IActionResult> GetPlayerStatistics(Guid userId, long afterDate)
        {
            _logger.LogDebug("GetPlayerStatistics for {userId} after {afterDate}", userId, afterDate);
            try
            {
                var answer = await _db.GetPlayerActivityStatistics(userId, new DateTime(afterDate, DateTimeKind.Utc));
                return Ok(answer);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost("/api/player/recordActivity")]
        public async Task<IActionResult> PlayerRecordActivity([FromBody] PlayerActivityRow model)
        {
            _logger.LogDebug("PlayerRecordActivity for {model}", model);

            if (model == null)
            {
                _logger.LogCritical("Could not record the players activity as the model was not bound, look for json error is {type}"
                    , nameof(PlayerActivityRow)
                    );
                return this.UnprocessableEntity("model not provided");
            }

            //use internal validation
            if (!ModelState.IsValid)
            {
                _logger.LogCritical("Sending data to PlayerRecordActivity failed as the model is not valid:{ModelState}",ModelState);
                return this.ValidationProblem(ModelState);
            }

            //test data validation
            if (!model.IsValid(out string validationError))
            {
                ModelState.AddModelError(string.Empty, validationError);
                _logger.LogCritical("Sending data to PlayerRecordActivity failed as the model is not valid:{ModelState}",validationError);

                return this.ValidationProblem(ModelState);
            }
            try
            {
                var success= await _db.SavePlayerRecordActivityAsync(playerId: model.PlayerId
                                             , clubId: model.PlayerClubId
                                             , matchId: model.MatchId
                                             , activity: model.Activity
                                             , recordedUtc: model.RecordedUtc).ConfigureAwait(false);
                if(success) return Ok();

                return NotFound($"Could not update activities for {model.PlayerId} in {model.MatchId} on {model.RecordedUtc}.");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }
        [HttpGet("/api/player/recordActivity/{playerId}/{matchId}")]
        public async Task<IActionResult> GetPlayerRecordActivity(Guid playerId, int matchId)
        {
            _logger.LogDebug("GetPlayerRecordActivity for {playerId} and {matchId}", playerId, matchId);
            try
            {
                List<PlayerActivityRow> result = await _db.GetPlayersActivity(playerId: playerId, matchId: matchId).ConfigureAwait(false);
                if (result.Count == 0)
                    return NotFound("player has no recorded activity");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost("/api/Player/UploadImage/{id:guid}")]
        public async Task<IActionResult> UploadImage(Guid id, IFormFile image)
        {
            _logger.LogDebug("UploadImage for {id}", id);
            using var memory = new MemoryStream();
            image.CopyTo(memory);
            try
            {

                bool success = await _db.UpdateProfileImageForAsync(userId: id
                                                                  , imageBytes: memory.ToArray()
                                                                  ).ConfigureAwait(false);

                if (success) return Ok();

                return NotFound("A user for the profile image was not found");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpPost("/api/Player/ProfileImage")]
        public async Task<IActionResult> PostProfileImage([FromBody] ProfileImage profileImage)
        {
            _logger.LogDebug("PostProfileImage for {profileImage}", profileImage);

            if (profileImage is null || profileImage.Id == Guid.Empty || string.IsNullOrEmpty(profileImage.Image))
                return NotFound("Data provided was not compatible with method");

            try
            {
                bool success = await _db.UpdateProfileImageForAsync(userId: profileImage.Id
                                                                  , imageBytes: profileImage.Image?.ToBytes()
                                                                  ).ConfigureAwait(false);

                if (success) return Ok();

                return NotFound("A user for the profile image was not found");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("/api/player/Acivity/{playerId}/{matchId}")]

        public async Task<IActionResult> GetPlayerMovements(Guid playerId, int matchId)
        {
            _logger.LogDebug("GetPlayerMovements for {playerId} and {matchId}", playerId, matchId);
            try
            {
                var result = await _db.GetPlayerMovementsAsync(playerId, matchId).ConfigureAwait(false);
                _logger.LogInformation("Recorded {count} entries for player in match {matchId}",result.Count,matchId);
                if (result.Count > 0)
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }

        }



        [HttpGet("/api/player/motionStatistics/{playerId}/{matchId}")]
        public async Task<IActionResult> GetPlayerMotionStatistics(Guid playerId, int matchId)
        {
            _logger.LogDebug("GetPlayerMotionStatistics for {playerId} and {matchId}", playerId, matchId);

            if (playerId == Guid.Empty || matchId == 0) return NotFound();

            try
            {
                var result = await _db.GetPlayerMotionStatisticsAsync(playerId, matchId).ConfigureAwait(false);
                _logger.LogDebug("Motion for player Sprints:{Sprints} AverageSpeed:{AverageSpeed} MedianSpeed:{MedianSpeed} and TopSpeed:{TopSpeed}"
                    , result.Sprints, result.AverageSpeed, result.MedianSpeed, result.TopSpeed);           

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }

        }
        [HttpPost("/api/Player/SavePlayerRecording")]
        public async Task<IActionResult> SavePlayerRecording([FromBody] RecordPlayerMovent model)
        {
            _logger.LogDebug("SavePlayerRecording for {PlayerId} on {Longitude}/{Latitude}", model.PlayerId, model.Longitude, model.Latitude);

            if (model.RecordedUtc < System.Data.SqlTypes.SqlDateTime.MinValue.Value || model.RecordedUtc > System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
            {
                _logger.LogWarning("In SavePlayerRecording RecordedUtc is not valid {RecordedUtc}, set to current time", model.RecordedUtc);
                model.RecordedUtc = DateTime.UtcNow;

            }

            if (!ModelState.IsValid || Request.HttpContext.Connection.RemoteIpAddress is null)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning(error.Exception,"Failed on the post as the model is not valid due to a {Type} with {Message}"
                        , error.Exception?.GetType().Name, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }
            try
            {
                //13 digits is 10 meter accuracy, my phone gives 13 so I add 2 more
                var success=await _db.SavePlayerRecording(model.PlayerId
                                           , model.MatchId
                                           , Math.Round(model.Latitude, 15)
                                           , Math.Round(model.Longitude, 15)
                                           , model.RecordedUtc
                                           ).ConfigureAwait(false);
                if(success) return Ok();
                
                return NotFound("Failed to record player location due to it not storing the data in the database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Cant process SavePlayerRecording due to a {Type} with:{Message}",ex.GetType().Name,ex.Message);
                return BadRequest(ex);
            }
        }
        [HttpPost("/api/Player/SavePlayerMotivation")]
        public async Task<IActionResult> SavePlayerMotivation([FromBody] PlayerGameMotivation model)
        {
            _logger.LogDebug("SavePlayerMotivation for :{model}", model);
            if (!ModelState.IsValid || Request.HttpContext.Connection.RemoteIpAddress is null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var success= await _db.SavePlayerMotivation(model.PlayerId, model.MatchId, model.PlayerMotivation, model.PlayerAttitude).ConfigureAwait(false);
                if(success) return Ok();

                //the UI expects the data to be returned
                return NotFound("Player did not update any rows");
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }

        // "api/Player/GetPlayerMotivation", maar korter geschreven
        [HttpPost("/" + MagicStrings.GetPlayerMotivation)]
        public async Task<IActionResult> GetPlayerMotivation([FromBody] PlayerGameMotivation model)
        {
            _logger.LogDebug("GetPlayerMotivation for :{player} in match {Match}", model?.PlayerId, model?.MatchId);

            if (!ModelState.IsValid || Request.HttpContext.Connection.RemoteIpAddress is null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var (PlayerAttitude, PlayerMotivation) = await _db.GetPlayerMotivationAsync(model!.PlayerId, model.MatchId).ConfigureAwait(false);
                model.PlayerMotivation = PlayerMotivation;
                model.PlayerAttitude = PlayerAttitude;
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }
        }

        [HttpGet("/api/Player/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogDebug("Get Player for {id}", id);
            if (Guid.Equals(id, Guid.Empty))
                return NotFound();


            try
            {
                var answer = await _db.GetPlayerAsync(id).ConfigureAwait(false);
                if (answer is not null)
                {
                    return Ok(answer);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return BadRequest(ex);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Player Model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            _logger.LogDebug("Post Player for {model}", Model.FullName);
            try
            {
                var playerId = await _db.UpdateAsync(Model).ConfigureAwait(false);
                if (Guid.Empty.Equals(playerId))
                {
                    return NotFound();
                }
                else
                {
                    return Ok(playerId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Cant process default Post handler due to a {Type} with:{Message}",ex.GetType().Name,ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPost("/api/player/UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePassword model)
        {
            _logger.LogDebug("UpdatePassword for {model}", model);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdatePassword for {model} fails as model is not valid", model);
                return BadRequest(modelState: ModelState);
            }
            try
            {
                var playerId = await _db.UpdateAsync(model);
                if (Guid.Empty.Equals(playerId))
                {
                     _logger.LogWarning("UpdatePassword for {model} fails as model is not mapped to a known user", model);
                    return NotFound("Did not find user");
                }
                else
                {
                    return Ok(playerId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Cant process UpdatePassword due to a {Type} with:{Message}",ex.GetType().Name,ex.Message);
                
                return BadRequest(ex);
            }

        }

        [HttpGet("/" + MagicStrings.PlayerNextMatch + "/{id}")]
        public async Task<IActionResult> PlayerNextMatch(string id)
        {
            _logger.LogDebug("PlayerNextMatch for {id}", id);
            try
            {
                if (Guid.TryParse(id, out var PlayerId))
                {
                    Match? match = await _db.GetNextMatchForPlayerAsync(PlayerId).ConfigureAwait(false);
                    if (match is not null)
                    {
                        _logger.LogInformation("Next match {MatchId} for player {id} returned {MatchDate}",match.MatchId,id, match.MatchDate);
                        return Ok(match);
                    }
                }

                return NotFound($"No match found for {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Cant process PlayerNextMatch due to a {Type} with:{Message}",ex.GetType().Name,ex.Message);
                return BadRequest(ex);
            }
        }
    }
}