using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LinnworksCategoryApi.Config;
using LinnworksCategoryApi.Headers;
using LinnworksCategoryApi.HttpClients;
using LinnworksCategoryApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace LinnworksCategoryApi.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryApiClient _categoryApiClient;
        private readonly IDashboardApiClient _dashboardApiClient;
        private readonly ILogger<CategoryController> _logger;
        private readonly IConfigurator<HttpResponseMessage, HttpResponse> _responseConfigurator;

        public CategoryController(
            ICategoryApiClient apiClient, 
            IDashboardApiClient dashboardApiClient, 
            ILogger<CategoryController> logger, 
            IConfigurator<HttpResponseMessage, HttpResponse> responseConfigurator)
        {
            _categoryApiClient = apiClient;
            _dashboardApiClient = dashboardApiClient;
            _logger = logger;
            _responseConfigurator = responseConfigurator;
        }

        [HttpGet("/api/GetCategories")]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var response = await _categoryApiClient.GetCategories(HttpContext, cancellationToken);

            _responseConfigurator.Apply(response, Response);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var categories = JsonSerializer.Deserialize<IEnumerable<Category>>(content);

            return new JsonResult(categories);
        }

        [HttpGet("/api/CategoriesWithProductCount")]
        public async Task<IActionResult> CategoriesWithProductCount(CancellationToken cancellationToken)
        {
            var categoriesWithProductCountSql =
                "SELECT p.CategoryName, p.CategoryId, Count(S.CategoryId) AS ProductCount " +
                "FROM StockItem AS S " +
                "RIGHT JOIN ProductCategories AS P " +
                "ON P.CategoryId = S.CategoryId " +
                "GROUP BY P.CategoryId, P.CategoryName";

            var response = await _dashboardApiClient.ExecuteCustomScriptQuery(HttpContext, cancellationToken, categoriesWithProductCountSql);

            _responseConfigurator.Apply(response, Response);

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;

            var queryResponse = JsonSerializer.Deserialize<DashboardResponse>(content);
            
            var categories = queryResponse.Results;

            return new JsonResult(categories);
        }

        [HttpPost("/api/CreateCategory")]
        public async Task<IActionResult> CreateCategory(Category category, CancellationToken cancellationToken)
        {
            var response = await _categoryApiClient.CreateCategory(category.CategoryName, HttpContext, cancellationToken);

            _responseConfigurator.Apply(response, Response);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var newCategory = JsonSerializer.Deserialize<Category>(content);

            return new JsonResult(newCategory);
        }

        [HttpPost("/api/UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(Category category, CancellationToken cancellationToken)
        {
            var response = await _categoryApiClient.UpdateCategory(category, HttpContext, cancellationToken);

            _responseConfigurator.Apply(response, Response);

            response.EnsureSuccessStatusCode();

            return new JsonResult(response.StatusCode);
        }

        [HttpPost("/api/DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(Category category, CancellationToken cancellationToken)
        {
            var response = await _categoryApiClient.DeleteCategoryById(category.CategoryId, HttpContext, cancellationToken);

            _responseConfigurator.Apply(response, Response);

            response.EnsureSuccessStatusCode();

            return new JsonResult(response.StatusCode);
        }
    }
}