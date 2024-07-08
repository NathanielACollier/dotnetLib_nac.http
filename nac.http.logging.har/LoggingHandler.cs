using nac.http.logging.har.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace nac.http.logging.har;

public class LoggingHandler : DelegatingHandler
{
    public static bool isEnabled = false; // programs will need to set this in the assembly to turn this on
    public static event Action<object, model.Entry> onMessage; // people will need to subscribe to this to get the log entries


    public LoggingHandler(HttpMessageHandler innerHandler): base(innerHandler){

    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if( isEnabled ){
            return handleLoggingSendingRequest(request, cancellationToken);
        }
         
        return base.SendAsync(request, cancellationToken);
    }


    private async Task<HttpResponseMessage> handleLoggingSendingRequest(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var entry = new model.Entry
        {
            StartedDateTime = DateTimeOffset.UtcNow,
            Pageref = "",
            Connection = "",
            ServerIpAddress = "",
            Timings = new model.Timings (),
            Cache = new Cache()
        };

        await WriteRequest(entry, request);

        try
        {
            var requestStopWatch = new System.Diagnostics.Stopwatch();
            requestStopWatch.Start();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            
            requestStopWatch.Stop();

            await WriteResponse(entry, response);

            CalculateTiming(entry, request, response, requestTime: requestStopWatch.Elapsed);
            
            return response;
        }
        catch (Exception ex)
        {
            // do we need to put exception details in HAR file????

            throw; // log and rethrow
        }
        finally
        {
            onMessage?.Invoke(this, entry);
        }
    }

    private void CalculateTiming(Entry entry, HttpRequestMessage request, HttpResponseMessage response, TimeSpan requestTime)
    {
        entry.Time = (long)Math.Ceiling( requestTime.TotalMilliseconds);
    }

    private async Task WriteResponse(Entry entry, HttpResponseMessage response)
    {
        var respModel = new model.Response
        {
            Status = (long)response.StatusCode,
            StatusText = response.ReasonPhrase,
            HttpVersion = "HTTP/" + response.Version.ToString(2)
        };
        entry.Response = respModel;
        
        // headers
        respModel.Headers = (from h in response.Headers
            from headerValue in h.Value
            select new model.Cooky
            {
                Name = h.Key,
                Value = headerValue
            }).ToArray();
        
        // cookies
        respModel.Cookies = ParseCookiesFromAllHeaders(respModel.Headers);
        
        // content
        respModel.BodySize = 0;
        if (response.Content != null)
        {
            respModel.BodySize = response.Content.Headers.ContentLength ?? 0;
            respModel.Content = await GenerateContentFromResponse(response);
        }

        respModel.RedirectUrl = response.Headers.Location?.OriginalString ?? "";
        respModel.HeadersSize = response.Headers.ToString().Length;
    }

    private async Task<Content> GenerateContentFromResponse(HttpResponseMessage response)
    {
        string responseBody = await response.Content.ReadAsStringAsync();

        var content = new model.Content
        {
            Size = response.Content.Headers.ContentLength ?? 0,
            Encoding = response.Content.Headers.ContentEncoding?.ToString() ?? "",
            MimeType = response.Content.Headers.ContentType?.ToString() ?? "",
            Text = responseBody
        };

        return content;
    }

    private async Task WriteRequest(Entry entry, HttpRequestMessage request)
    {
        var reqModel = new model.Request
        {
            Method = request.Method.Method,
            Url = request.RequestUri.OriginalString,
            HttpVersion = "HTTP/" + request.Version.ToString(2)
        };
        entry.Request = reqModel;
        
        // headers
        reqModel.Headers = (from h in request.Headers
            from headerValue in h.Value
            select new model.Cooky
            {
                Name = h.Key,
                Value = headerValue
            }).ToArray();

        reqModel.Cookies = ParseCookiesFromAllHeaders(reqModel.Headers);
        
        // querystring
        reqModel.QueryString = GenerateRequestQueryParams(request);

        reqModel.BodySize = 0;
        // reuqest body
        if (request.Content != null)
        {
            var postDataResult = await GeneratePostDataFromRequest(request);
            reqModel.PostData = postDataResult;
            reqModel.BodySize = request.Content.Headers.ContentLength ?? 0;
        }

        // Got header size and body size from here: https://stackoverflow.com/questions/46508589/how-to-calculate-the-size-of-a-httprequestmessage
        reqModel.HeadersSize = request.Headers.ToString().Length;
    }

    private Cooky[] ParseCookiesFromAllHeaders(Cooky[] reqModelHeaders)
    {
        var cookieHeader = reqModelHeaders.FirstOrDefault(h => string.Equals(h.Name, "Set-Cookie"));

        var cookieList = new List<model.Cooky>();

        if (cookieHeader == null)
        {
            return cookieList.ToArray();
        }

        foreach (var cookieStr in cookieHeader.Value.Split(';'))
        {
            var parts = cookieStr.Split('=');
            cookieList.Add(new model.Cooky
            {
                Name = parts[0].Trim(),
                Value = parts[1].Trim()
            });
        }

        return cookieList.ToArray();
    }

    private async Task<model.PostData> GeneratePostDataFromRequest(HttpRequestMessage request)
    {
        string bodyContent = await request.Content.ReadAsStringAsync();
        
        var post = new model.PostData
        {
            MimeType = request.Content.Headers.ContentType.ToString(),
            Text = bodyContent,
            Params = default(object[])
        };

        return post;
    }

    private Cooky[] GenerateRequestQueryParams(HttpRequestMessage request)
    {
        var queryParams = System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query);

        var result = from key in queryParams.AllKeys
            select new model.Cooky
            {
                Name = key,
                Value = queryParams.Get(key)
            };

        return result.ToArray();
    }
    
    
    
}