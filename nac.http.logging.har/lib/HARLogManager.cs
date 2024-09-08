using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace nac.http.logging.har.lib;

public class HARLogManager
{
    static int fileWriteTimeout = (int)(new TimeSpan(0,5,0)).TotalMilliseconds;
    static ReaderWriterLock locker = new ReaderWriterLock();
    
    private string filePath;
    private List<model.Entry> harEntries = new();

    public HARLogManager(string httpArchiveFilePath)
    {
        this.filePath = httpArchiveFilePath;
        
        LoggingHandler.isEnabled = true;
        LoggingHandler.onMessage += async (_sender, _entry) =>
        {
            harEntries.Add(_entry);
            await WriteToDisk();
        };
    }


    private Task WriteToDisk(){
        return Task.Run(() => {
            try
            {
                locker.AcquireWriterLock(millisecondsTimeout: fileWriteTimeout); //You might wanna change timeout value 
                writeOutAllHrEntries();
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        });
        
    }

    private void writeOutAllHrEntries(){
        if(!harEntries.Any()){
            return; // no har entries, nothing to do
        }

        string jsonEntries = utility.BuildHARFileJSON(harEntries);
        System.IO.File.WriteAllText(path: this.filePath, contents: jsonEntries);
    }


}
