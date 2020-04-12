using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinnworksCategoryApi.Config;
using LinnworksCategoryApi.Headers;
using LinnworksCategoryApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinnworksCategoryApi.HttpClients
{
    public class CategoryApiClient : ICategoryApiClient
    {
        private readonly IConfigurator<HttpContext, HttpRequestMessage> _headersConfigurator;
        private readonly HttpClient _httpClient;
        private readonly CategoryApiConfig _categoryApiConfig;
        private readonly ILogger<CategoryApiClient> _logger;


        public CategoryApiClient(IHttpClientFactory clientFactory,
            IConfigurator<HttpContext, HttpRequestMessage> headersConfigurator,
            IOptions<CategoryApiConfig> categoryApiConfig, 
            ILogger<CategoryApiClient> logger)
        {
            _categoryApiConfig = categoryApiConfig.Value;

            _headersConfigurator = headersConfigurator;
           
            _logger = logger;

            _httpClient = clientFactory.CreateClient();
            _httpClient.Timeout = new TimeSpan(0, 0, _categoryApiConfig.Timeout);
            _httpClient.BaseAddress = new Uri(_categoryApiConfig.BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> GetCategories(HttpContext requestContext, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _categoryApiConfig.GetCategories);

            _headersConfigurator.Apply(requestContext, request);

            return await _httpClient.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> CreateCategory(string categoryName, HttpContext requestContext, CancellationToken cancellationToken)
        {
            var resourceAddress = string.Format(_categoryApiConfig.CreateCategory, categoryName);

            var request = new HttpRequestMessage(HttpMethod.Post, resourceAddress);

            _headersConfigurator.Apply(requestContext, request);

            return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> UpdateCategory(Category category, HttpContext requestContext, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _categoryApiConfig.UpdateCategory)
            {
                Content = new StringContent($"category={{\"CategoryId\":\"{category.CategoryId}\",\"CategoryName\":\"{category.CategoryName}\"}}", 
                        Encoding.UTF8, 
                        "application/x-www-form-urlencoded")
            };

            _headersConfigurator.Apply(requestContext, request);

            return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> DeleteCategoryById(Guid id, HttpContext requestContext, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _categoryApiConfig.DeleteCategoryById)
            {
                Content = new StringContent($"categoryId={id}", Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            _headersConfigurator.Apply(requestContext, request);

            return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        }
    }
}