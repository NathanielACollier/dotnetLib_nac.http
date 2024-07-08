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
}