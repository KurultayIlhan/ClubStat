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
        private static Session? _session;
        protected ApiBasedFactory(IConfiguration configuration, IHttpClientFactory clientFactory)
        {

            var settings = new Settings.ApiSettings() { ApiKey = "123", Url = "https://ilhankurultay-001-site1.btempurl.com" };
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

        public string? LastResponse { get; private set; }

        /// <summary>
        /// Get bytes as an asynchronous operation.
        /// </summary>
        /// <param name="apiUrl">The API URL.</param>
        /// <returns>A Task&lt;System.Byte[]&gt; representing the asynchronous operation.</returns>
        protected async Task<byte[]> GetBytesAsync(string apiUrl)
        {
            var client = _clientFactory.CreateClient(Settings.Url);
            client.BaseAddress =new Uri(Settings.Url, uriKind: UriKind.Absolute);

            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {

                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
                if (_session is not null)
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
                }
            }
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                LastResponse = response.IsSuccessStatusCode
                    ? response.ReasonPhrase
                    : $"{apiUrl} generated a {response.StatusCode}:{response.ReasonPhrase}";

                if (response.IsSuccessStatusCode)
                {
                    if(_session is not null) _session.Expiry= DateTime.Now.AddMinutes(30);

                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch (Exception ex) 
            {
                Walter.Inverse.GetLogger("GetBytesAsync")?.LogException(ex);
            }
            return [];

        }

        public async Task<TResult?> GetAsync<TResult>(string endppoint) where TResult : class
        {
            var client = _clientFactory.CreateClient(Settings.Url);
            client.BaseAddress =new Uri(Settings.Url, uriKind: UriKind.Absolute);

            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
                if (_session is not null)
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
                }
            }


            using var answer = await client.GetAsync(endppoint).ConfigureAwait(false);
            
            LastResponse = answer.IsSuccessStatusCode
                ? answer.ReasonPhrase
                : $"{endppoint} generated a {answer.StatusCode}:{answer.ReasonPhrase}";

            if (answer.IsSuccessStatusCode)
            {
                if(_session is not null) _session.Expiry= DateTime.Now.AddMinutes(30);
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
            client.BaseAddress =new Uri(Settings.Url, uriKind: UriKind.Absolute);
            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
                if (_session is not null)
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
                }
            }


            using var answer = await client.GetAsync(endppoint).ConfigureAwait(false);
            LastResponse = answer.IsSuccessStatusCode
                ? answer.ReasonPhrase
                : $"{endppoint} generated a {answer.StatusCode}:{answer.ReasonPhrase}";
            if (answer.IsSuccessStatusCode)
            {
                if(_session is not null) _session.Expiry= DateTime.Now.AddMinutes(30);
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
            client.BaseAddress =new Uri(Settings.Url, uriKind: UriKind.Absolute);
            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
                if (_session is not null)
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
                }
            }
            try
            {
                var postModel = new StringContent(question.AsJson(), encoding: Encoding.UTF8, "application/json");
                using var answer = await client.PostAsync(endppoint, postModel).ConfigureAwait(false);
                LastResponse = answer.ReasonPhrase;
                if (answer.IsSuccessStatusCode)
                {
                    if (answer.Headers.TryGetValues("Authorization", out var auth))
                    {
                        if (auth.FirstOrDefault() is string token)
                        { 
                            _session= new Session() { 
                                Token = token
                                , Expiry= DateTime.Now.AddMinutes(30)
                            };                        
                        }
                    }


                    if (await answer.Content.ReadAsStringAsync().ConfigureAwait(false) is string json
                        && !string.IsNullOrEmpty(json)
                        && json.IsValidJson<TResult>(MagicTypes.JsonTypeInfo<TResult>(), out TResult? value))
                        return value;
                }
                else
                {
                    LastResponse = $"{endppoint} generated a {answer.StatusCode}:{answer.ReasonPhrase}";
                }
            }catch (Exception ex) 
            {
                Walter.Inverse.GetLogger("Login")?.LogException(ex);
            }
            return null;

        }
        public async Task<bool> WriteDataAsync(string endppoint, IAsJson question) 
        {
            var client = _clientFactory.CreateClient(Settings.Url);
            client.BaseAddress =new Uri(Settings.Url, uriKind: UriKind.Absolute);
            if (!client.DefaultRequestHeaders.Contains(MagicStrings.API_HEADER))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(MagicStrings.API_HEADER, Settings.ApiKey);
                if (_session is not null)
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
                }
            }

            var postModel = new StringContent(question.AsJson(), encoding: Encoding.UTF8, "application/json");
            using var answer = await client.PostAsync(endppoint, postModel).ConfigureAwait(false);
            
            LastResponse =answer.IsSuccessStatusCode
                            ? answer.ReasonPhrase
                            : $"{endppoint} generated a {answer.StatusCode}:{answer.ReasonPhrase}";
            if (answer.IsSuccessStatusCode && _session is not null)
            {
                _session.Expiry = DateTime.Now.AddMinutes(30);
            }
            return answer.IsSuccessStatusCode;
        }
    }
}
