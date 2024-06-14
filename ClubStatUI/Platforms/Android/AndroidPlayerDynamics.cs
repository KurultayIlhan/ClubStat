// ***********************************************************************
// Assembly         : ClubStatUI
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Thu 16-May-2024
// ***********************************************************************
// <copyright file="AndroidPlayerDynamics.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Factories;
using ClubStat.Infrastructure.Models;

namespace ClubStatUI.Platforms.Android
{
    class AndroidPlayerDynamics : IPlayerDynamics
    {
        private readonly IMessageDialog _messageService;
        private readonly ILoginFactory _loginFactory;

        public AndroidPlayerDynamics(IMessageDialog messageService, ILoginFactory loginFactory)
        {
            this._messageService = messageService;
            _loginFactory = loginFactory;
            Member = _loginFactory.CurrentUser;
            LastKnownLocation = new PlayerDynamicsLocation(0, 0, DateTime.MinValue, 0,Member?.UserId ?? Guid.Empty);
        }

        public bool IsInGame { get; set; }
        public LoggedInUser? Member { get; set; }
        public Match? Match { get; set; }

        public PlayerDynamicsLocation LastKnownLocation { get; private set; }


        public async Task<PlayerDynamicsLocation> GetPlayerDynamicsLocation()
        {
            try
            {
                if (!IsInGame)
                {
                    return LastKnownLocation;
                }

                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    //ui interactivity must be called on the UI thread
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        // Request the permissions as it is not already granted
                        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    });
                }

                if (status == PermissionStatus.Granted)
                {
                    var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best))
                                                             .ConfigureAwait(false);
                    if (location is null)
                    {
                        await Geolocation.Default.GetLastKnownLocationAsync().ConfigureAwait(false);
                    }

                    if (location is not null)
                    {

                        LastKnownLocation = new PlayerDynamicsLocation(Convert.ToDecimal(location.Latitude)
                                                                     , Convert.ToDecimal(location.Longitude)
                                                                     , DateTime.Now
                                                                     , Match?.MatchId ?? 0
                                                                     , Member?.UserId ?? Guid.Empty
                                                                     );
                    }
                    
                }

                return LastKnownLocation;
                // Can be that sometimes the gps is busy, but it is asking for the last known location. It will never fail perm otherwise it will throw an ex.
                //return await GetPlayerDynamicsLocation().ConfigureAwait(false);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                _messageService.ShowMessage($"Your gps is not supported on this device, I'm sorry but you cant use it on this device. Your device reported:{fnsEx.Message}");
                throw;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                _messageService.ShowMessage($"Handle not enabled on device exception. Your device reported:{fneEx.Message}");
                throw;
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                _messageService.ShowMessage($"Handle permission exception. Your device reported:{pEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                // Unable to get location
                _messageService.ShowMessage($"Unable to get location. Your device reported:{ex.Message}");
                throw;
            }
        }
    }
}
