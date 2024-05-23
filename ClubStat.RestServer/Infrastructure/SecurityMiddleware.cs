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

namespace ClubStat.RestServer.Infrastructure
{
    public class SecurityMiddleware
    {
        private static MemoryCacheEntryOptions _cashingOptions = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(30) };
        private readonly RequestDelegate _next;

        public SecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // checking the route and applying security measures    
            // Perform some security checks on header
            if ( context.Connection.RemoteIpAddress is null ||
                (context.Request.Path.HasValue
                && string.IsNullOrEmpty(context.Request.Path.Value) 
                && !context.Request.Path.Value.StartsWith("/swagger") 
                && !IsAuthenticated(context)))
            {
                //do not allow random access
                await  context.Response.BodyWriter.WriteAsync($"Access to {context.Request.Path} is not allowed outside of the Application".ToBytes()).ConfigureAwait(false);
                context.Response.StatusCode = 401; // Unauthorized
                return; // Stop further processing
            }
            
            var key = string.Concat(context.Request.GetDisplayUrl(), context.Connection.RemoteIpAddress?.ToString());
            var _memory =context.RequestServices.GetRequiredService<IMemoryCache>();
            if (_memory.TryGetValue<int>(key, out var attempts))
            {
                attempts++;
                await Task.Delay(attempts * 1000).ConfigureAwait(false);//become slower with eached failed loggin

            }

            // Call the next middleware in the pipeline
            await _next(context).ConfigureAwait(false);

            //add delay on future request if within sliding expiration
            if (context.Response.StatusCode >= 400)
            { 
                _memory.Set(key, attempts, _cashingOptions);
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

            if (!context.Request.Headers.ContainsKey(MagicStrings.API_HEADER)) return false;
            var key = context.RequestServices.GetRequiredService<ClubStat.Infrastructure.Settings.ApiSettings>();
            return context.Request.Headers.TryGetValue(MagicStrings.API_HEADER, out var header) && string.Equals(header.ToString(), key.ApiKey, StringComparison.OrdinalIgnoreCase);
        }
    }
}
