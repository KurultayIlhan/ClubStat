// ***********************************************************************
// Assembly         : ClubStat.Infrastructure
// Author           : Ilhan
// Created          : Sat 11-May-2024
//
// Last Modified By : Ilhan
// Last Modified On : Sun 12-May-2024
// ***********************************************************************
// <copyright file="LoginFactory.cs" company="Private eigendom Ilhan Kurultay">
//     2024  © Ilhan Kurultay All rights reserved
// </copyright>
// <summary>
// TASK 7: Integrate Login with REST API
// </summary>
// ***********************************************************************
using System.Text;
namespace ClubStat.Infrastructure.Factories
{
    abstract class ApiBasedFactory
    {
        //use it to manage Http request to API Rest Endpoints
        private readonly IHttpClientFactory _clientFactory;

        protected ApiBasedFactory(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            var settings = new Settings.ApiSettings() { ApiKey = Guid.Empty.ToString(), Url = "https://localhost:5000" };
            //load setting from binding
            configuration.Bind("Api", settings);

            //make sure the settings are loaded or generate a exception auto logged
            Walter.Guard.EnsureNotNullOrEmpty(settings.ApiKey);
            Walter.Guard.EnsureNotNullOrEmpty(settings.Url);

            //save the settings
            Settings = settings;
            _clientFactory = clientFactory;
        }

        protected ApiSettings Settings { get; }

        /// <summary>
        /// Get bytes as an asynchronous operation.
        /// </summary>
        /// <param name="apiUrl">The API URL.</param>
        /// <returns>A Task&lt;System.Byte[]&gt; representing the asynchronous operation.</returns>
        protected async Task<byte[]> GetBytesAsync(string apiUrl)
        {
            var client = _clientFactory.CreateClient(Settings.Url);
            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
            }
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();

        }

        public async Task<TResult?> GetAsync<TResult>(string endppoint) where TResult : class
        {
            var client = _clientFactory.CreateClient(Settings.Url);
            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
            }


            using var answer = await client.GetAsync(endppoint).ConfigureAwait(false);
            if (answer.IsSuccessStatusCode)
            {
                if (await answer.Content.ReadAsStringAsync().ConfigureAwait(false) is string json
                    && json.IsValidJson<TResult>(MagicTypes.JsonTypeInfo<TResult>(), out TResult? value)
                    )
                    return value;
            }

            return null;
        }

        public async Task<TResult?> GetAsync<TResult>(string endppoint, System.Text.Json.Serialization.Metadata.JsonTypeInfo<TResult> typeInfo) where TResult : class
        {
            var client = _clientFactory.CreateClient(Settings.Url);
            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
            }


            using var answer = await client.GetAsync(endppoint).ConfigureAwait(false);
            if (answer.IsSuccessStatusCode)
            {
                if (await answer.Content.ReadAsStringAsync().ConfigureAwait(false) is string json
                    && json.IsValidJson<TResult>(typeInfo, out TResult? value)
                    )
                    return value;
            }

            return null;
        }
       
        public async Task<TResult?> PostAsync<TResult>(string endppoint, IAsJson question) where TResult : class
        {
            var client = _clientFactory.CreateClient(Settings.Url);
            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
            }

            var postModel = new StringContent(question.AsJson(), encoding: Encoding.UTF8, "application/json");
            using var answer = await client.PostAsync(endppoint, postModel).ConfigureAwait(false);
            if (answer.IsSuccessStatusCode)
            {
                if (await answer.Content.ReadAsStringAsync().ConfigureAwait(false) is string json
                    && json.IsValidJson<TResult>(MagicTypes.JsonTypeInfo<TResult>(), out TResult? value))
                    return value;
            }

            return null;

        }
        public async Task<bool> WriteDataAsync(string endppoint, IAsJson question) 
        {
            var client = _clientFactory.CreateClient(Settings.Url);
            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
            }

            var postModel = new StringContent(question.AsJson(), encoding: Encoding.UTF8, "application/json");
            using var answer = await client.PostAsync(endppoint, postModel).ConfigureAwait(false);
            return answer.IsSuccessStatusCode;
        }
    }
}
