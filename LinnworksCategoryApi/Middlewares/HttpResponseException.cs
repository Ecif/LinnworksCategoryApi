using System;

namespace LinnworksCategoryApi.Middlewares
{
    public class HttpResponseException : Exception
    {
        public int Status { get; set; } = 500;

        public object Value { get; set; }
    }
}