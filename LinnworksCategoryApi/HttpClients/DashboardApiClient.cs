using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinnworksCategoryApi.Config;
using LinnworksCategoryApi.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinnworksCategoryApi.HttpClients
{
    public class DashboardApiClient : IDashboardApiClient
    {
        private readonly IConfigurator<HttpContext, HttpRequestMessage> _headersConfigurator;
        private readonly HttpClient _httpClient;
        private readonly DashboardApiConfig _dashboardApiConfig;
        private readonly ILogger<DashboardApiClient> _logger;


        public DashboardApiClient(IHttpClientFactory clientFactory,
            IConfigurator<HttpContext, HttpRequestMessage> headersConfigurator, 
            IOptions<DashboardApiConfig> dashboardApiConfig, ILogger<DashboardApiClient> logger)
        {
            _dashboardApiConfig = dashboardApiConfig.Value;

            _headersConfigurator = headersConfigurator;
            
            _logger = logger;

            _httpClient = clientFactory.CreateClient();
            _httpClient.Timeout = new TimeSpan(0, 0, _dashboardApiConfig.Timeout);
            _httpClient.BaseAddress = new Uri(_dashboardApiConfig.BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<HttpResponseMessage> ExecuteCustomScriptQuery(HttpContext requestContext, CancellationToken cancellationToken, string customScript)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, _dashboardApiConfig.ExecuteCustomScriptQuery)
            {
                Content = new StringContent($"script={customScript}",
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded")
            };

            _headersConfigurator.Apply(requestContext, request);

            return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}