// ***********************************************************************
// Assembly         : 
// Author           : Ilhan
// Created          : Tue 11-Jun-2024
//
// Last Modified By : Ilhan
// Last Modified On : Tue 11-Jun-2024
// ***********************************************************************
// <copyright file="OnlineDetector.cs" company="Ilhan">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary>
// Test if the device has internet access
// </summary>
// ***********************************************************************
using ClubStat.Infrastructure;
using ClubStat.Infrastructure.Infrastructure;

namespace ClubStatUI.Infrastructure
{
    internal class OnlineDetector : IOnlineDetector
    {
        private readonly IMessageDialog _messageDialog;
        private static bool _permissionMissingMessageShown = false;

        public OnlineDetector(IMessageDialog messageDialog)
        {
            _messageDialog = messageDialog;
        }
        public bool IsOnline()
        {
            try
            {
                return Connectivity.NetworkAccess == NetworkAccess.Internet; // Use Maui's Connectivity class to check network access
            }
            catch (PermissionException)
            {
                if (_messageDialog != null && !_permissionMissingMessageShown)
                {
                    _messageDialog.ShowMessage("No network access permission was given");
                    _permissionMissingMessageShown = true;
                }

                try
                {
                    var ping = new System.Net.NetworkInformation.Ping();
                    var reply = ping.Send("www.google.com");
                    return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
