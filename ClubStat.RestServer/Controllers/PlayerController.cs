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

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            if(Guid.Equals(id, Guid.Empty))
                return NotFound();



            var answer = await _db.GetPlayerAsync(id).ConfigureAwait(false);
            if (answer is not null)
            {
                return Ok(answer);
            }

            return BadRequest();

        }

        [HttpPost]
        public async Task<IActionResult> Post(Player Model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            var playerId= await _db.UpdateAsync(Model).ConfigureAwait(false);
            if (Guid.Empty.Equals(playerId))
            { 
                return NotFound();
            }
            else
            {
                return Ok(playerId);
            }
        }

        [HttpPost("api/player/UpdatePassword")]
        public async Task<IActionResult> Post(UpdatePassword model)
        { 
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }
            var playerId= await _db.UpdateAsync(model);
            if (Guid.Empty.Equals(playerId))
            { 
                return NotFound();
            }
            else
            {
                return Ok(playerId);
            }
        
        }
    }
}
