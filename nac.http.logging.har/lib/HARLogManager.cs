using System;
using System.Collections.Generic;
using System.Linq;

namespace nac.http.logging.har.lib;

public class HARLogManager : IDisposable
{
    private string filePath;
    private List<model.Entry> harEntries = new();

    public HARLogManager(string httpArchiveFilePath)
    {
        this.filePath = httpArchiveFilePath;
        
        LoggingHandler.isEnabled = true;
        LoggingHandler.onMessage += (_sender, _entry) =>
        {
            harEntries.Add(_entry);
        };
    }

    public void Dispose()
    {
        if(!harEntries.Any()){
            return; // no har entries, nothing to do
        }

        string jsonEntries = utility.BuildHARFileJSON(harEntries);
        System.IO.File.WriteAllText(path: this.filePath, contents: jsonEntries);
    }
}
