// ***********************************************************************
// Assembly         : ClubStat.RestServer
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Tue 14-May-2024
// ***********************************************************************
// <copyright file="LoginController.cs" company="ClubStat.RestServer">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Models;
using ClubStat.RestServer.Infrastructure;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ClubStat.RestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private static MemoryCacheEntryOptions _cashingOptions = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(1) };
        private readonly ILogger _logger;
        private readonly IMemoryCache _memory;
        private readonly DbHelper _db;
        public LoginController(DbHelper db, ILogger<LoginController> logger, IMemoryCache memory)
        {
            this._db = db;
            this._logger = logger;
            _memory = memory;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LogInUser model)
        {
            if (ModelState.IsValid || Request.HttpContext.Connection.RemoteIpAddress is null)
            {
                return BadRequest(ModelState);
            }

            var key = string.Concat(this.HttpContext.Request.GetDisplayUrl(), "FAILED_LOGIN", HttpContext.Connection.RemoteIpAddress?.ToString());
            var _memory = HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            if (_memory.TryGetValue<int>(key, out var attempts) && attempts > 3)
            {
                return Forbid("To many failed login attepts, please wait some time and try again");
            }
            _logger.LogInformation("Processing request login for user {loginName}", model?.Username);
            if (model is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _db.LoginUserAsync(model).ConfigureAwait(false);

            if (result.UserType != UserType.None)
            {

                //if login ok remove it if exists                
                _memory.Remove(key);

                return Ok(result);

            }


            attempts++;

            _memory.Set<int>(key, attempts, _cashingOptions);

            return NotFound();
        }
    }
}
