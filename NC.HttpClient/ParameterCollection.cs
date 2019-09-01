using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NC.HttpClient
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
    }
}
