using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LinnworksCategoryApi.Models;
using Microsoft.AspNetCore.Http;

namespace LinnworksCategoryApi.HttpClients
{
    public interface ICategoryApiClient
    {
        Task<HttpResponseMessage> GetCategories(HttpContext requestContext, CancellationToken cancellationToken);
        Task<HttpResponseMessage> CreateCategory(string categoryName, HttpContext requestContext, CancellationToken cancellationToken);
        Task<HttpResponseMessage> UpdateCategory(Category category, HttpContext requestContext, CancellationToken cancellationToken);
        Task<HttpResponseMessage> DeleteCategoryById(Guid id, HttpContext requestContext, CancellationToken cancellationToken);
    }
}