using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PolicyTrackerWebClient.Service
{
    /// <summary>
    /// Calls the Web API. The bearer token is set per request (not on the shared HttpClient)
    /// so concurrent users never send each other's token.
    /// </summary>
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _client;

        public HttpClientService()
        {
            _client = HttpClientProvider.Client;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        public async Task<TR> GetAsync<TR>(string url, string token = null)
        {
            using (var request = Build(HttpMethod.Get, url, null, token))
            using (var response = await _client.SendAsync(request).ConfigureAwait(false))
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                EnsureSuccess(response, content);
                return JsonConvert.DeserializeObject<TR>(content);
            }
        }

        public async Task<TR> PostAsync<T, TR>(string url, T obj, string token = null)
        {
            using (var request = Build(HttpMethod.Post, url, obj, token))
            using (var response = await _client.SendAsync(request).ConfigureAwait(false))
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                EnsureSuccess(response, content);
                return response.StatusCode == HttpStatusCode.NoContent ? default(TR) : JsonConvert.DeserializeObject<TR>(content);
            }
        }

        public async Task PutAsync<T>(string url, T obj, string token = null)
        {
            using (var request = Build(HttpMethod.Put, url, obj, token))
            using (var response = await _client.SendAsync(request).ConfigureAwait(false))
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                EnsureSuccess(response, content);
            }
        }

        public async Task DeleteAsync(string url, string token = null)
        {
            using (var request = Build(HttpMethod.Delete, url, null, token))
            using (var response = await _client.SendAsync(request).ConfigureAwait(false))
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                EnsureSuccess(response, content);
            }
        }

        private static HttpRequestMessage Build(HttpMethod method, string url, object body, string token)
        {
            var request = new HttpRequestMessage(method, url);
            if (token.IsNotNullOrEmpty())
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (body != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            return request;
        }

        private static void EnsureSuccess(HttpResponseMessage response, string content)
        {
            if (response.IsSuccessStatusCode) return;

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedAccessException("Authorization has been denied for this request.");
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Forbidden:
                    string message = null;
                    try { message = JsonConvert.DeserializeObject<string>(content); }
                    catch (Exception) { message = string.IsNullOrWhiteSpace(content) || content.Length > 300 ? null : content.Trim(); }
                    throw new APIResponseException(message ?? "Bad Request!");
                default:
                    throw new APIResponseException("Internal server error!");
            }
        }
    }
}
