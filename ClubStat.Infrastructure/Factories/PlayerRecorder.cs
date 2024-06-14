// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : Thu 16-May-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Sat 01-Jun-2024
// ***********************************************************************
// <copyright file="PlayerRecorder.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure.Infrastructure;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Diagnostics;

using Walter;

namespace ClubStat.Infrastructure.Factories
{


    internal sealed class PlayerRecorder : ApiBasedFactory, IPlayerRecorder, IAsyncDisposable
    {

        ConcurrentQueue<(Player player, Match match, PlayerDynamicsLocation location)> _locationQueue;

        private volatile bool _uploading;
        private readonly IMemoryCache _memory;
        IOnlineDetector? _onlineDetector;
        private bool _disposedValue;

        public PlayerRecorder(IConfiguration configuration, IHttpClientFactory httpClientFactory, IMemoryCache memory, IServiceProvider lazyLoader)
            : base(configuration, httpClientFactory)
        {
            _onlineDetector = lazyLoader.GetService<IOnlineDetector>();
            _locationQueue = new();
            _memory = memory;
        }

        public async Task<bool> RecordActivity(Player player, Match match, PlayerActivities activity)
        {
            var model = new PlayerActivityRow()
            {
                Activity = activity,
                MatchId = match.MatchId,
                PlayerClubId = player.ClubId,
                PlayerId = player.UserId,
                RecordedUtc = DateTime.UtcNow,
            };
            var success=await base.WriteDataAsync(MagicStrings.PlayerRecordActivityUrl, model).ConfigureAwait(false);
            if (!success && !string.IsNullOrEmpty(base.LastResponse)) 
            {
                Trace.WriteLine(base.LastResponse);
            }
            return success;

        }

        public bool HasData()=>!_locationQueue.IsEmpty;

        public void RecordLocation(Player player, Match match, PlayerDynamicsLocation location)
        {
            

            _locationQueue.Enqueue((player, match, location));
            if (!_uploading)
            {
                _uploading = true;
                //start this task as a long running background task that may affect UI state
                Task.Run(UploadLocationsAsync);
            }
        }

        private bool AsumeIsOnline()
        {
            if (_onlineDetector is not null)
            {
                return _onlineDetector.IsOnline();
            }
            return true;
        }

        public DateTime LastUpdate { get; private set; }
        public DateTime? LastRestError { get; private set; }

        /// <summary>
        /// Upload locations as an asynchronous operation from the queue.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks><see cref="LastRestError" /> for any issues during uploads</remarks>
        public async Task UploadLocationsAsync()
        {
            var logger= Walter.Inverse.GetLogger("IPlayerRecorder.UploadLocationsAsync");
            try
            {
                while (AsumeIsOnline() && _locationQueue.TryDequeue(out var item))
                {
                    logger?.LogInformation("Item dequeued");

                    var record = new RecordPlayerMovent(item.player.UserId
                                                    , item.match.MatchId
                                                    , Convert.ToDouble(item.location.Lat)
                                                    , Convert.ToDouble(item.location.Lng)
                                                    , item.location.Recorded.ToUniversalTime()
                                                    );

                    var success = await base.WriteDataAsync(MagicStrings.RecordPlayerLocation, record);

                    if (!success)
                    {
                        logger?.LogInformation("Item dequeued but not saved");
                    }
                    //try saving for one minute if not saved
                    if (!success && (DateTime.Now - item.location.Recorded) < TimeSpan.FromMinutes(1))
                    {
                        LastRestError = DateTime.Now;
                        Inverse.GetLogger("PlayerRecorder.UploadLocationsAsync")?
                                        .Lazy()
                                        .LogInformation("Failed to upload record location for {UserId} in match {MatchId} due to a {LastResponse}"
                                                    , item.player.UserId, item.match.MatchId,base.LastResponse??string.Empty);
                        

                        RecordLocation(item.player, item.match, item.location);//enqueue it again
                    }
                    else
                    {
                        LastUpdate = item.location.Recorded.ToLocalTime();
                    }
                    try
                    {
                        //invalidate memory while recording
                        var keys = new[]{
                              $"api/club/LastKnownLocation/{item.player.ClubId}/{item.match.MatchId}"
                             ,$"api/match/LastKnownLocation/{item.match.MatchId}/{item.player.UserId}"
                             ,$"GetPlayerStatistics-{item.player.UserId}"
                             ,$"api/player/motionStatistics/{item.player.UserId}/{item.match.MatchId}"
                             ,MagicStrings.GetPlayerMovementsInGame(item.player.UserId, item.match.MatchId)
                            };
                        foreach (var key in keys)
                        {
                            if (_memory.TryGetValue(key, out _)) _memory.Remove(key);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger?.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Walter.Inverse.GetLogger("PlayerRecorder.UploadLocationsAsync")?.LogException(ex);
            }
            _uploading = false;

        }

        public async Task<List<PlayerMovement>?> GetPlayerMovementsAsync(Player player, Match match)
        {

            var useMemory = match.MatchDate.AddMinutes(100) < DateTime.Now;
            string endpoint = MagicStrings.GetPlayerMovementsInGame(player.UserId, match.MatchId);

            if (useMemory && _memory.TryGetValue<List<PlayerMovement>>(endpoint, out var memory) && memory is not null)
                return memory;


            var answer = await base.GetAsync<List<PlayerMovement>>(endpoint).ConfigureAwait(false);
            if (answer != null && useMemory)
            {
                _memory.Set(endpoint, answer, new DateTimeOffset(DateTime.Now.AddDays(1)));
            }
            return answer;
        }
        public async Task<List<PlayerActivityRow>> GetRecordActivity(Player player, Match match)
        {
            var url = $"/api/player/recordActivity/{player.UserId}/{match.MatchId}";
            var noCashing = match.MatchDate > DateTime.Now && match.MatchDate < DateTime.Now.AddMinutes(110);
            if (!noCashing && _memory.TryGetValue<List<PlayerActivityRow>>(url, out var memory) && memory is not null && memory.Count > 0)
                return memory;
            
            var answer= await base.GetAsync<List<PlayerActivityRow>>(url,PlayerActivityRowsJsonContext.Default.ListPlayerActivityRow).ConfigureAwait(false);
            if (answer is not null && answer.Count > 0)
            {
                if (!noCashing)
                {
                    _memory.Set(url, answer, new DateTimeOffset(DateTime.Now.AddMinutes(5)));
                }

                return answer;
            }

            return new();
        }
        public async Task<PlayerStatistics> GetPlayerStatisticsAsync(Player player)
        {
            var key = $"GetPlayerStatistics-{player.UserId}";
            if (_memory.TryGetValue<PlayerStatistics>(key, out var memory) && memory is not null)
                return memory;

            int year = DateTime.Now.Year;

            // Check if the given date is before August 1st
            if (DateTime.Now.Month < 8)
            {
                // If before August 1st, the season started in the previous year
                year--;
            }

            var answer = await GetPlayerStatisticsAsyc(player, new DateTime(year, 8, 1));


            return answer;

        }
        public async Task<PlayerStatistics> GetPlayerStatisticsAsyc(Player player, DateTime afterDate)
        {
            string endpoint = MagicStrings.GetPlayerStatistics(player.UserId,afterDate);
            if (_memory.TryGetValue<PlayerStatistics>(endpoint, out var memory) && memory is not null) { return memory; }

            var asnwer = await base.GetAsync<PlayerStatistics>(endpoint);
            if (asnwer is not null && asnwer.TillUtc!= DateTime.MinValue)
            {
                _memory.Set(endpoint, asnwer, new DateTimeOffset(DateTime.Now.AddDays(1)));
            }
            return asnwer ?? new();//if no statisctics returned

        }

        public async Task<PlayerMotionStatistics> GetPlayerMotionStatisticsAsync(Player user, int matchId)
        {
            var url = $"api/player/motionStatistics/{user.UserId}/{matchId}";
            if (_memory.TryGetValue<PlayerMotionStatistics>(url, out var memory) && memory is not null) return memory;

            var answer = await base.GetAsync<PlayerMotionStatistics>(url, PlayerMotionStatisticsJsonContext.Default.PlayerMotionStatistics).ConfigureAwait(false);

            if (answer != null)
            {
                _memory.Set(url, answer, new DateTimeOffset(DateTime.Now.AddDays(1)));
            }

            //if null give null answer, perhaps player did not play in the match
            answer ??= new PlayerMotionStatistics()
            {
                MatchId = matchId,
                PlayerId = user.UserId,
                AverageSpeed = 0,
                MedianSpeed = 0,
                Sprints = 0,
                TopSpeed = 0
            };
            //todo ask database for the player stats

            return answer;
        }

        public async Task<PlayerMovement?> GetPlayersLastKnownLocation(Player player, Match match)
        {
            if(player is null|| match is null) return null;

            var url = $"api/match/LastKnownLocation/{match.MatchId}/{player.UserId}";
            if (_memory.TryGetValue<PlayerMovement>(url, out var memory) || memory is not null) return memory;

            var answer = await base.GetAsync<PlayerMovement>(url, PlayerMovementJsonContext.Default.PlayerMovement).ConfigureAwait(false);
            if (answer != null && answer.Speed > 0)
            {
                _memory.Set(url, answer, new DateTimeOffset(DateTime.Now.AddMinutes(10)));
            }

            return answer;
        }

        public async Task<List<PlayerMovement>?> GetTeamsLastKnownLocation(int clubId, int matchId)
        {
            var url = $"api/club/LastKnownLocation/{clubId}/{matchId}";
            if (_memory.TryGetValue<List<PlayerMovement>>(url, out var memory) && memory is not null && memory.Count > 0)
                return memory;

            var answer = await base.GetAsync<List<PlayerMovement>>(url, PlayerMovementsJsonContext.Default.ListPlayerMovement).ConfigureAwait(false);
            if (answer != null && answer.Count > 0)
            {
                _memory.Set(url, answer, new DateTimeOffset(DateTime.Now.AddMinutes(10)));
            }

            return answer;

        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _onlineDetector = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PlayerRecorder()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await UploadLocationsAsync();
            Dispose(disposing: true);
        }
    }
}
