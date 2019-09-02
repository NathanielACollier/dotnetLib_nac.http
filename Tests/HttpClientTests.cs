using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HttpClientTests
    {
        private string baseUrl = "api/general/";

        [TestMethod]
        public async Task TestSimpleGet()
        {
            var result = await lib.shared.client.GetJSONAsync<string>($"{baseUrl}/test1");

            Assert.IsTrue(string.Equals(result, "Hello World!"));
        }
    }
}
