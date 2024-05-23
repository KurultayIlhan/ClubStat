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

        public async Task<TResult?> GetAsync<TResult>(string endppoint) where TResult : class, new()
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

    }
}
