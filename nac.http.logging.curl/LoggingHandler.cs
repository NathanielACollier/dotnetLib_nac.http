using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace nac.http.logging.curl;

public class LoggingHandler : DelegatingHandler
{
    public static bool isEnabled = false; // programs will need to set this in the assembly to turn this on
    public static event Action<object, model.LogEntry> onMessage; // people will need to subscribe to this to get the log entries


    public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
    {

    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (isEnabled)
        {
            return handleLoggingSendingRequest(request, cancellationToken);
        }
        else
        {
            return base.SendAsync(request, cancellationToken);
        }

    }


    private async Task<HttpResponseMessage> handleLoggingSendingRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {

        var entry = new model.LogEntry();

        entry.Request_CURLCommand = await formCurlCommand(request);

        try
        {
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.Content != null)
            {
                entry.ResponseContent = await response.Content.ReadAsStringAsync();
            }

            entry.request = request;
            entry.response = response;

            return response;
        }
        catch (Exception ex)
        {
            entry.SendException = ex;

            throw; // we just want to log it, but then need to rethrow
        }
        finally
        {
            onMessage?.Invoke(this, entry);
        }


    }


    private async Task<string> formCurlCommand(HttpRequestMessage request)
    {
        var curlCmdSB = new StringBuilder();
        // see curl documencation: https://curl.haxx.se/docs/manpage.html

        curlCmdSB.Append($"curl -X {request.Method}");

        foreach (var h in request.Headers)
        {
            string val = string.Join(",", h.Value);

            curlCmdSB.Append($" -H \"{h.Key}: {val}\" ");
        }

        if (request.Content != null)
        {
            string content = await request.Content.ReadAsStringAsync();
            curlCmdSB.Append($" -d \'{content}\' ");
        }

        curlCmdSB.Append($" {request.RequestUri} ");

        return curlCmdSB.ToString();
    }


}


