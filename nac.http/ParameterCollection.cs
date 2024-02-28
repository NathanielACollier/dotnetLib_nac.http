using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace nac.http
{
    // original from: https://stackoverflow.com/questions/17096201/build-query-string-for-system-net-httpclient-get
    public class ParameterCollection
    {
        private Dictionary<string, string> _parms = new Dictionary<string, string>();

        public ParameterCollection Add(string key, string val)
        {
            if (_parms.ContainsKey(key))
            {
                throw new InvalidOperationException(string.Format("The key {0} already exists.", key));
            }
            _parms.Add(key, val);
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var kvp in _parms)
            {
                if (sb.Length > 0) { sb.Append("&"); }
                sb.AppendFormat("{0}={1}",
                    WebUtility.UrlEncode(kvp.Key),
                    WebUtility.UrlEncode(kvp.Value));
            }
            return sb.ToString();
        }


        public static ParameterCollection ParseQueryString(string queryStr)
        {
            var result = new ParameterCollection();
            queryStr = queryStr.Trim();

            if(string.IsNullOrWhiteSpace(queryStr))
            {
                return result; // empty string results in empty parameter collection
            }

            if( queryStr[0] == '?')
            {
                queryStr.Substring(1); // skip the question mark
            }

            if (queryStr.Contains("?"))
            {
                throw new Exception($"Query string can only contain one question mark (Which is the optional first character).  A question mark was seen at position {queryStr.IndexOf('?')}.  This is not allowed.");
            }

            var pairs = queryStr.Split('&');
            int pairCount = 0; // this is for better error messages
            foreach( var p in pairs)
            {
                var keyVal = p.Split('=');
                if( keyVal.Length != 2)
                {
                    throw new Exception($"Error with key value pair #{pairCount}.  It did not contain an equals or something else is malformed about it");
                }
                var key = WebUtility.UrlDecode(keyVal[0]);
                var val = WebUtility.UrlDecode(keyVal[1]);
                result.Add(key, val);
                ++pairCount;
            }

            return result;
        }
    }
}
