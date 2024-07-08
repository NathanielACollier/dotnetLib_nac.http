using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests;

[TestClass]
public class __MSTest_Setup
{
    private static nac.Logging.Logger log = new();

    private static List<nac.http.logging.har.model.Entry> harEntries = new();

    [AssemblyInitialize()]
    public static async Task MyTestInitialize(TestContext testContext)
    {
        nac.Logging.Appenders.Debug.Setup();

        // curl logging
        nac.http.logging.curl.LoggingHandler.isEnabled = true;
        nac.http.logging.curl.LoggingHandler.onMessage += (__s, _args) =>
        {
            log.Debug(_args.ToString());
        };

        // har file logging
        nac.http.logging.har.LoggingHandler.isEnabled = true;
        nac.http.logging.har.LoggingHandler.onMessage += (__s, _args) => {
            harEntries.Add(_args);
        };

        log.Info("Tests Starting");

    }

    [AssemblyCleanup]
    public static void TearDown()
    {
        // save the har files now that all tests have ran
        string harJSON = nac.http.logging.har.lib.utility.BuildHARFileJSON(harEntries);
        System.IO.File.WriteAllText("http.har", harJSON);

    }
}
