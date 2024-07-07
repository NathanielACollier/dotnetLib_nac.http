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

    [AssemblyInitialize()]
    public static async Task MyTestInitialize(TestContext testContext)
    {
        nac.Logging.Appenders.Debug.Setup();

        nac.http.logging.curl.LoggingHandler.isEnabled = true;
        nac.http.logging.curl.LoggingHandler.onMessage += (__s, _args) =>
        {
            log.Debug(_args.ToString());
        };

        log.Info("Tests Starting");

    }

    [AssemblyCleanup]
    public static void TearDown()
    {


    }
}
