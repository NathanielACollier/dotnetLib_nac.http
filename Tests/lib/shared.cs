using System;
namespace Tests.lib
{
    public static class shared
    {
        private static NC.HttpClient.HttpClient __client;
        public static NC.HttpClient.HttpClient client
        {
            get { return __client ?? (__client = new NC.HttpClient.HttpClient("https://localhost:50001")); }
        }
    }
}
