using System;
namespace Tests.lib
{
    public static class shared
    {
        private static nac.http.HttpClient __client;
        public static nac.http.HttpClient client
        {
            get
            {
                if (__client == null)
                {
                    __client = new nac.http.HttpClient("http://localhost:5000", useWindowsAuth: false);
                }

                return __client;
            }
        }
    }
}
