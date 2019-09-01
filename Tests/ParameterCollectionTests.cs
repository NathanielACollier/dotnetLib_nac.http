using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSimpleQueryString()
        {
            var queryStr = new NC.HttpClient.ParameterCollection()
                            .Add("test", "Great Barrier Biscuit Factory")
                            .ToString();

            Assert.IsTrue(string.Equals(queryStr, "test=Great+Barrier+Biscuit+Factory"));
        }
    }
}
