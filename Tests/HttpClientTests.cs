using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class HttpClientTests
    {
        private string baseUrl = "api/general";

        [TestMethod]
        public async Task TestSimpleGet()
        {
            var result = await lib.shared.client.GetJSONAsync<string>($"{baseUrl}/test1");

            Assert.IsTrue(string.Equals(result, "Hello World!"));
        }


        [TestMethod]
        public async Task TestSimplePost()
        {
            var result = await lib.shared.client.PostJSONAsync<System.Text.Json.Nodes.JsonNode>($"{baseUrl}/testPostHash",
                new Dictionary<string, object>
                {
                    {"prop1", "Thanksgiving!" }
                });

            var prop1 = result["prop1"].Deserialize<string>();
            Assert.IsTrue(string.Equals(prop1, "Thanksgiving!"));
        }

        [TestMethod]
        public async Task EmptyHttpClient_GetGoogle()
        {
            var http = new nac.http.HttpClient();
            var resp = await http.GetJSONAsync<string>("https://google.com");
            
            Assert.IsTrue(!string.IsNullOrWhiteSpace(resp));
        }
    }
}
