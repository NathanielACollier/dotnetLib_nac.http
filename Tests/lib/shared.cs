using System;
namespace Tests.lib
{
    public static class shared
    {
        private static nac.http.HttpClient __client;
        public static nac.http.HttpClient client
        {
            get { return __client ?? (__client = new nac.http.HttpClient("https://localhost:5001")); }
        }
    }
}
