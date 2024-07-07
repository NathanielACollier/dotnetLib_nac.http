using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace nac.http.logging.curl.model;

public class LogEntry
{
    public string Request_CURLCommand { get; set; }

    public string ResponseContent { get; set; }

    public HttpResponseMessage response { get; set; }
    public HttpRequestMessage request { get; set; }

    public Exception SendException { get; set; }


    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("##### CURL #####");

        sb.AppendLine("Request:");
        sb.AppendLine("---------------");
        sb.AppendLine(this.Request_CURLCommand);
        sb.AppendLine("---------------");

        if (this.SendException == null)
        {
            sb.AppendLine($"Response(code: {this.response.StatusCode}):");
            sb.AppendLine("---------------");
            sb.AppendLine(this.ResponseContent);

        }
        else
        {
            sb.AppendLine("!!! SEND FAILURE !!!");
            sb.AppendLine("---");
            sb.AppendLine("Exception:");
            sb.AppendLine("---");
            sb.AppendLine(this.SendException.ToString());
        }

        sb.AppendLine("################");

        return sb.ToString();
    }

}
