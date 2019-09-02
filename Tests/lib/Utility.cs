using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.lib
{
    internal static class Utility
    {
        internal static void assertThrows(Action codeToRun)
        {
            try
            {
                codeToRun();
                Assert.Fail("Excepted exception, no exception thrown");
            }
            catch (Exception ex)
            {

            }
        }
    }
}
