// ***********************************************************************
// Assembly         : ClubStat.RestServer
// Author           : Ilhan
// Created          : Sun 12-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Sun 12-May-2024
// ***********************************************************************
// <copyright file="SecurityMiddleware.cs" company="ClubStat.RestServer">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary>
// Task 8 : Add security middleware on web api
// </summary>
// ***********************************************************************
using ClubStat.Infrastructure;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Memory;

using System.Diagnostics;
using System.Net;

namespace ClubStat.RestServer.Infrastructure
{
    public class SecurityMiddleware
    {
        private static MemoryCacheEntryOptions _cashingOptions = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(30) };
        private readonly RequestDelegate _next;
        ILogger _logger;


        public SecurityMiddleware(RequestDelegate next, ILogger<SecurityMiddleware> logger)
        {
            _next = next;
            _logger = logger;

        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogTrace("Seurity handeling for request from {ip} for {endpoint}",context.Connection.RemoteIpAddress, context.Request.Path);

            try
            {

                // checking the route and applying security measures    
                // Perform some security checks on header
                if (context.Connection.RemoteIpAddress is null ||
                    (context.Request.Path.HasValue
                    && string.IsNullOrEmpty(context.Request.Path.Value)
                    && !context.Request.Path.Value.StartsWith("/swagger")
                    && !IsAuthenticated(context)))
                {
                    _logger.LogTrace("request from {ip} for {endpoint} was rejected", context.Connection.RemoteIpAddress, context.Request.Path);

                    //do not allow random access
                    await context.Response.BodyWriter.WriteAsync($"Access to {context.Request.Path} is not allowed outside of the Application".ToBytes())
                                                      .ConfigureAwait(false);

                    context.Response.StatusCode = 401; // Unauthorized
                    return; // Stop further processing
                }

                var key = string.Concat(context.Request.GetDisplayUrl(), context.Connection.RemoteIpAddress?.ToString());
                var memory = context.RequestServices.GetRequiredService<IMemoryCache>();
                if (memory.TryGetValue<int>(key, out var attempts))
                {
                    _logger.LogInformation("request from {ip} for {endpoint} is delaid to force a slowdown of requests due to bad {attempts} attempts", context.Connection.RemoteIpAddress, context.Request.Path, attempts);
                    attempts++;
                    await Task.Delay(TimeSpan.FromSeconds(attempts)).ConfigureAwait(false);//become slower with eached failed loggin

                }
                _logger.LogDebug("Request from {ip} for {endpoint} was accepted", context.Connection.RemoteIpAddress, context.Request.Path);

                var sw = Stopwatch.StartNew();
                // Call the next middleware in the pipeline
                await _next(context).ConfigureAwait(false);
                sw.Stop();

                LogLevel logLevel = context.Response.StatusCode == 200 ? LogLevel.Debug : LogLevel.Warning;

                _logger.Log(logLevel, "Request from {ip} for {endpoint} generated a {status} (processing time {Elapsed})"
                                , context.Connection.RemoteIpAddress
                                , context.Request.Path
                                , context.Response.StatusCode
                                , sw.Elapsed
                                );

                //add delay on future request if within sliding expiration if not authorised
                if (context.Response.StatusCode >= 400
                    && context.Response.StatusCode < 404
                    && (!context.Request.Path.HasValue || context.Request.Path.Value.Contains(MagicStrings.LoginApi, StringComparison.OrdinalIgnoreCase))
                    && (context.Request.Path.HasValue && context.Request.Path.Value.Contains("favicon", StringComparison.OrdinalIgnoreCase))
                    )
                {
                    memory.Set(key, attempts, _cashingOptions);
                }
            }
            catch (Exception ex)
            {                

                _logger.LogCritical(ex,"Processing {path} caused a {ex} with message:{message}",context.Request.Path, ex.GetType().Name,ex.Message);
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var response = new { message = $"An {ex.GetType().Name} occurred while processing your request." };
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                }

            }

        }


        /// <summary>
        /// Determines whether the specified context is authenticated.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if the specified context is authenticated; otherwise, <c>false</c>.</returns>
        private bool IsAuthenticated(HttpContext context)
        {
            // Implement your authentication logic here
            // For example:

            if (!context.Request.Headers.ContainsKey(MagicStrings.API_HEADER))
            {
                _logger.LogInformation("request from {ip} for {endpoint} was rejected due to missing {header} header",context.Connection.RemoteIpAddress, context.Request.Path,MagicStrings.API_HEADER);
                return false;
            }

            var key = context.RequestServices.GetRequiredService<ClubStat.Infrastructure.Settings.ApiSettings>();
            var hasKey = context.Request.Headers.TryGetValue(MagicStrings.API_HEADER, out var header) && string.Equals(header.ToString(), key.ApiKey, StringComparison.OrdinalIgnoreCase);
            _logger.LogInformation("request from {ip} for {endpoint} was rejected due to invalid {header} key",context.Connection.RemoteIpAddress, context.Request.Path,header);
            bool authorised = context.Request.Path.HasValue;
           
            if (authorised)
            {
                authorised = context.Request.Headers.TryGetValue("Authorization", out var authHeader);

                if (authorised)
                {
                    var elements= authHeader.ToString().Split(' ');
                    authorised = elements.Length == 2 && string.Equals(elements[0], "Bearer", StringComparison.OrdinalIgnoreCase);
                    if (!authorised)
                    {
                        _logger.LogInformation("request from {ip} for {endpoint} was rejected due to invalid Authorization header",context.Connection.RemoteIpAddress, context.Request.Path);
                        return false;
                    }
                    var service = context.RequestServices.GetRequiredService<ISessionService>();
                    var token = elements[1];
                    if (service.ValidateSession(token) is ClubStat.Infrastructure.Models.Session session)
                    {
                        _logger.LogDebug("User from {ip} is valid any may access {endpoint}",context.Connection.RemoteIpAddress,context.Request.Path);
                        return true;
                    }
                    else 
                    {
                        _logger.LogInformation("request from {ip} for {endpoint} was rejected due to invalid Authorization token",context.Connection.RemoteIpAddress, context.Request.Path);
                    }

                }
                else
                {
                    _logger.LogInformation("request from {ip} for {endpoint} was rejected due to missing Authorization header",context.Connection.RemoteIpAddress, context.Request.Path);
                }
            }

            return hasKey && authorised;
        }
    }
}
