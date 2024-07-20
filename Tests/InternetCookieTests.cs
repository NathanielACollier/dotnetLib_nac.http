using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests;

[TestClass]
public class InternetCookieTests
{


    [TestMethod]
    public async Task GetGoogleHome()
    {
        string html = await lib.shared.client.GetJSONAsync<string>("https://google.com");
        
        Assert.IsTrue(!string.IsNullOrWhiteSpace(html));
    }
    
}