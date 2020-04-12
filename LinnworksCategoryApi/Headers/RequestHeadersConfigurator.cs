using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Primitives;

namespace LinnworksCategoryApi.Headers
{
    public class RequestHeadersConfigurator : IConfigurator<HttpContext, HttpRequestMessage>
    {
        public void Apply(HttpContext source, HttpRequestMessage target)
        {
            var typedHeaders = source.Request.GetTypedHeaders();

            var acceptHeaders = typedHeaders.Accept?.Select(x => x.MediaType).ToArray() ?? new StringSegment[] { };

            foreach (StringSegment acceptHeader in acceptHeaders)
            {
                target.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader.Value));
            }

            var acceptEncoding = typedHeaders.AcceptEncoding.Select(x => x.Value) ?? new StringSegment[] { };

            foreach (StringSegment stringSegment in acceptEncoding)
            {
                target.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue(stringSegment.Value));
            }

            if (source.Request.Headers.TryGetValue("Authorization", out var authorizationToken))
            {
                target.Headers.Authorization = new AuthenticationHeaderValue(authorizationToken.ToString());
            }


        }
    }
}