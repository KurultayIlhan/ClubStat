﻿// ***********************************************************************
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
            LastKnownLocation = new PlayerDynamicsLocation(0, 0, DateTime.MinValue, _loginFactory.CurrentUser?.UserId ?? Guid.Empty);
        }

        public bool IsInGame { get; set; }

        public PlayerDynamicsLocation LastKnownLocation { get; private set; }


        public async Task<PlayerDynamicsLocation> GetPlayerDynamicsLocation()
        {
            try
            {
                if (!IsInGame)
                {
                    return LastKnownLocation;
                }

                var location = await Geolocation.Default.GetLastKnownLocationAsync().ConfigureAwait(false);
                if (location is not null && _loginFactory.CurrentUser is not null && _loginFactory.CurrentUser.UserType == UserType.Player)
                {
                    LastKnownLocation = new PlayerDynamicsLocation(Convert.ToDecimal(location.Latitude), Convert.ToDecimal(location.Longitude), DateTime.Now, _loginFactory.CurrentUser.UserId);
                }


                // Can be that sometimes the gps is busy, but it is asking for the last known location. It will never fail perm otherwise it will throw an ex.
                return await GetPlayerDynamicsLocation().ConfigureAwait(false);
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