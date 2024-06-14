// ***********************************************************************
// Assembly         : 
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Tue 11-Jun-2024
// ***********************************************************************
// <copyright file="AndroidMessage.cs" company="Ilhan">
//     Copyright (c) Ilhan. All rights reserved.
// </copyright>
// <summary>
// Thread-safe way of showing a message on Android
// </summary>
// ***********************************************************************
using Android.Widget;

using ClubStat.Infrastructure;

namespace ClubStatUI.Platforms.Android
{
    /// <summary>
    /// Class AndroidMessage will show a message mased on a string.
    /// Implements the <see cref="IMessageDialog" />
    /// </summary>
    /// <seealso cref="IMessageDialog" />
    class AndroidMessage : IMessageDialog
    {
        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">The message to show.</param>
        public void ShowMessage(string message)
        {
            //check if on background thread if so call on main thread
            if (MainThread.IsMainThread)
            {
                Toast.MakeText(Platform.CurrentActivity, message, ToastLength.Short);
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
            {
                Toast.MakeText(Platform.CurrentActivity, message, ToastLength.Short);
            });
            }

        }
    }
}
