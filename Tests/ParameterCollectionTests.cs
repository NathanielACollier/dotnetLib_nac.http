using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSimpleQueryString()
        {
            var queryStr = new nac.http.ParameterCollection()
                            .Add("test", "Great Barrier Biscuit Factory")
                            .ToString();

            Assert.IsTrue(string.Equals(queryStr, "test=Great+Barrier+Biscuit+Factory"));
        }


        [TestMethod]
        public void TestCreateFromEmptyString()
        {
            var result = nac.http.ParameterCollection.ParseQueryString("");

            Assert.IsTrue(result.ToString().Length == 0);
        }

        [TestMethod]
        public void TestMalformedQueryStr()
        {
            lib.Utility.assertThrows(() =>
            {
                var result = nac.http.ParameterCollection.ParseQueryString("??"); // bad query string
            });
        }


        [TestMethod]
        public void TestThatQueryStringGetsRecreated()
        {
            var queryStr = "knights=of&the=seven&kingdom=game&of=thrones";

            var builder = nac.http.ParameterCollection.ParseQueryString(queryStr);

            Assert.IsTrue(string.Equals(queryStr, builder.ToString()));
        }


        [TestMethod]
        public void TestForMalformedKeyValuePair()
        {
            var querystr1 = "knights";
            lib.Utility.assertThrows(() =>
            {
                nac.http.ParameterCollection.ParseQueryString(querystr1);
            });
            var querystr2 = "betty&boopity&boop";
            lib.Utility.assertThrows(() =>
            {
                nac.http.ParameterCollection.ParseQueryString(querystr2);
            });
        }


    }
}
