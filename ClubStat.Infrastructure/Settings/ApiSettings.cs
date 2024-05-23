// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan Kurultay
// Created          : Tue 27-Feb-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Tue 27-Feb-2024
// ***********************************************************************
// <copyright file="ApiSettings.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace ClubStat.Infrastructure.Settings
{
    /// <summary>
    /// Class ApiSettings is populated via configuration file and used by the services and factories talking with the server.
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Gets the URL for the application to talk with.
        /// </summary>
        /// <value>The URL.</value>
        public required string Url { get; set; }
        /// <summary>
        /// Gets the API key that is used to talk with the API.
        /// </summary>
        /// <remarks>
        /// This key could be private to a user allowing
        /// licensing as well as protect the api from
        /// scrubbing by detecting manipulation
        /// </remarks>
        /// <value>The API key.</value>
        public required string ApiKey { get; set; }
    }
}
