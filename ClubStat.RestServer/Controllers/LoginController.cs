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
// <summary>
// Manage user login as well as Authorization ticket
// </summary>
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
        private IMemoryCache _memory;
        private readonly ISessionService _session;
        private readonly DbHelper _db;
        public LoginController(DbHelper db, ILogger<LoginController> logger, IMemoryCache memory,ISessionService session)
        {
            this._db = db;
            this._logger = logger;

            _memory = memory;//used to count failed login attempts per minute
            _session = session;//used to manage login sessions without a cookie
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LogInUser model)
        {
            if (!ModelState.IsValid || Request.HttpContext.Connection.RemoteIpAddress is null || model is null)
            {
                _logger.LogInformation("Login model is not valid");
                return BadRequest(ModelState);
            }

            var key = string.Concat(this.HttpContext.Request.GetDisplayUrl(), "FAILED_LOGIN", HttpContext.Connection.RemoteIpAddress?.ToString());
            _memory ??= HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            if (_memory.TryGetValue<int>(key, out var attempts) && attempts > 3)
            {
                _logger.LogInformation("To many failed login attepts,  tried {attempts} 3 are allowed per {timespan}"
                                    , attempts
                                    ,_cashingOptions.SlidingExpiration
                                    );

                return Forbid("To many failed login attepts, please wait some time and try again");
            }


            _logger.LogInformation("Processing request login for user {loginName}", model?.Username);
            var result = await _db.LoginUserAsync(model!).ConfigureAwait(false);

            if (result.UserType != UserType.None)
            {
                //create a random token based on the unique Trace Identifiere 
                var token = HttpContext.TraceIdentifier.AsShaHash(HashMethod.SHA384, 26);
                var session= _session.CreateSession(token);
                session.UserId = result.UserId;

                Response.Headers.Authorization= session.Token; 
                //if login ok remove it if exists                
                _memory.Remove(key);
                _logger.LogTrace("Login for {UserName} OK",model!.Username);
                return Ok(result);

            }


            attempts++;
            _logger.LogTrace("Login for {UserName} failed {attempts} times",model!.Username,attempts);
            _memory.Set<int>(key, attempts, _cashingOptions);

            return NotFound("users with provided credentials is not found");
        }
    }
}
