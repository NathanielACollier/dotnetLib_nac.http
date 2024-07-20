using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using nac.http.logging.har.model;

namespace nac.http.logging.har.lib;

public static class utility
{
    public static string BuildHARFileJSON(List<model.Entry> entries)
    {
        var root = BuildHARFileModel(entries);

        string jsonText = System.Text.Json.JsonSerializer.Serialize(root, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return jsonText;
    }

    public static model.HARFileRoot BuildHARFileModel(List<model.Entry> entries)
    {
        var pages = WorkOutUniquePages(entries);
        
        var root = new model.HARFileRoot
        {
            Log = new model.Log
            {
                Version = "1.2",
                Creator = new model.Browser
                {
                    Version = "1.0",
                    Name = typeof(LoggingHandler).FullName
                },
                Entries = entries.ToArray(),
                Pages = pages
            }
        };

        return root;
    }

    private static Page[] WorkOutUniquePages(List<Entry> entries)
    {
        var apiCallPage = new model.Page
        {
            Id = "api_call_1",
            StartedDateTime = entries.First().StartedDateTime,
            Title = entries.First().Request.Url,
            PageTimings = new PageTimings
            {
                OnLoad = entries.Select(e=>e.Time).Sum()
            }
        };
        
        // set the page ref for all entries
        foreach (var e in entries)
        {
            e.Pageref = apiCallPage.Id;
        }

        return new model.Page[] { apiCallPage };
    }


    /**
     # Parse Cookies Documentation
     + See stack overflow: https://stackoverflow.com/questions/28979882/parsing-cookies/78773639#78773639
     ## Microsoft Cookie Parser
     + There is an internal cookieParser that Microsoft maintains System.Net.CookieParser: https://github.com/dotnet/runtime/blob/ef5664875a63000b853edfeda909c410c6927b92/src/libraries/Common/src/System/Net/CookieParser.cs#L517-L517
     + That can be called via SetCookies on CookieContainer: https://github.com/dotnet/runtime/blob/ef5664875a63000b853edfeda909c410c6927b92/src/libraries/System.Net.Primitives/src/System/Net/CookieContainer.cs#L1028-L1028
     
     ## Third Party Cookie Parser from github.com/RyuaNerin
     + See: https://gist.github.com/RyuaNerin/a03b5f6ee6866d571ebd9714c1043b32
     
     */

    public static System.Net.CookieContainer ParseCookieHeader(string cookieHeader, System.Uri uri)
    {
        var cookies = new System.Net.CookieContainer();
        cookies.SetCookies(uri: uri,
            cookieHeader: cookieHeader);

        return cookies;
    }
    
    
}