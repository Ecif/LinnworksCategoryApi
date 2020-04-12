using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace LinnworksCategoryApi.Headers
{
    public class ResponseHeadersConfigurator : IConfigurator<HttpResponseMessage, HttpResponse>
    {
        public void Apply(HttpResponseMessage source, HttpResponse target)
        {
            if (source.Content.Headers.TryGetValues("Content-Type", out IEnumerable<string> responseContentType))
            {
                target.ContentType = string.Join(";", responseContentType);
            }
        }
    }
}