using System;
using System.Net.Http;

namespace PolicyTrackerWebClient.Infrastructure
{
    public class HttpClientProvider
    {
        private static readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };

        public static HttpClient Client => _client;
    }
}
