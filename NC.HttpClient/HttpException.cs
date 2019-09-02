using System;
using System.Net;

namespace NC.HttpClient
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpException(HttpStatusCode _code, string _errorResponse) : base(_errorResponse)
        {
            this.StatusCode = _code;
        }

        public override string ToString()
        {
            return $"HTTP_EXCEPTION: [{StatusCode}] {Message}";
        }

    }
}
