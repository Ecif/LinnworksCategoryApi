using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LinnworksCategoryApi.HttpClients
{
    public interface IDashboardApiClient
    {
        Task<HttpResponseMessage> ExecuteCustomScriptQuery(HttpContext requestContext, CancellationToken cancellationToken, string customScript);

    }
}