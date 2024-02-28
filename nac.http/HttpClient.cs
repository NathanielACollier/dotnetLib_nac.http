using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace nac.http
{
    public class HttpClient
    {
        private System.Net.Http.HttpClient http;

        public HttpClient(string baseUrl, bool useWindowsAuth = true, TimeSpan? timeout = null)
        {
            if (useWindowsAuth)
            {
                http = new System.Net.Http.HttpClient(new System.Net.Http.HttpClientHandler
                {
                    UseDefaultCredentials = true
                });
            }
            else
            {
                http = new System.Net.Http.HttpClient();
            }

            if (timeout.HasValue)
            {
                http.Timeout = timeout.Value;
            }

            http.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            http.DefaultRequestHeaders.Accept.Clear();
            http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }





        public async Task<T> PostJSONAsync<T>(object obj)
        {
            return await PostJSONAsync<T>("", obj);
        }

        public async Task<T> PostJSONAsync<T>(string relativeUrl, object obj)
        {
            // see: https://stackoverflow.com/questions/6117101/posting-jsonobject-with-httpclient-from-web-api
            var jsonStr = jsonSerialize(obj);
            
            var jsonStrContent = new StringContent(jsonStr, Encoding.UTF8, "application/json");

            var response = await this.http.PostAsync(relativeUrl, jsonStrContent);

            return await ProcessHttpResponse<T>(response);
        }


        private string jsonSerialize(object obj)
        {
            string jsonText = System.Text.Json.JsonSerializer.Serialize(value: obj, options: new JsonSerializerOptions
            {
                
            });

            return jsonText;
        }

        private T jsonDeserialize<T>(string jsonText)
        {
            T result = System.Text.Json.JsonSerializer.Deserialize<T>(json: jsonText, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }

        public async Task<T> PostFormUrlEncodeAsync<T>(string relativeUrl, IEnumerable<KeyValuePair<string, string>> values, HttpStatusCode[] successCodes = null)
        {
            HttpContent formContent = new FormUrlEncodedContent(values);

            var response = await this.http.PostAsync(relativeUrl, formContent);
            return await ProcessHttpResponse<T>(response, successCodes: successCodes);
        }


        public async Task<T> PutFormUrlEncodeAsync<T>(string relativeUrl, IEnumerable<KeyValuePair<string, string>> values, HttpStatusCode[] successCodes = null)
        {
            HttpContent formContent = new FormUrlEncodedContent(values);

            var response = await this.http.PutAsync(relativeUrl, formContent);
            return await ProcessHttpResponse<T>(response, successCodes: successCodes);
        }


        public async Task<T> DeleteAsync<T>(string relativeUrl)
        {
            var response = await this.http.DeleteAsync(relativeUrl);
            return await ProcessHttpResponse<T>(response);
        }


        public async Task<T> GetJSONAsync<T>(string relativeUrl, Dictionary<string, string> queryParameters)
        {
            // note this: https://stackoverflow.com/questions/3865975/namevaluecollection-to-url-query

            string baseQuery = "";
            string baseRelativeUrl = "";

            if (relativeUrl.Contains("?"))
            {
                var pieces = relativeUrl.Split('?');
                if (pieces.Length != 2)
                {
                    throw new Exception($"Relative URL: [{relativeUrl}] is malformed, maybe it contains more than one '?' character?  Something is wrong with it...");
                }

                baseQuery = pieces[1];
                baseRelativeUrl = pieces[0];
            }
            else
            {
                baseRelativeUrl = relativeUrl;
            }

            var values = ParameterCollection.ParseQueryString(baseQuery);
            foreach (var pair in queryParameters)
            {
                values.Add(pair.Key, pair.Value);
            }

            return await GetJSONAsync<T>(baseRelativeUrl + '?' + values.ToString());
        }


        public async Task<T> GetJSONAsync<T>(string relativeUrl)
        {
            var response = await this.http.GetAsync(relativeUrl);

            return await ProcessHttpResponse<T>(response);
        }




        private async Task<T> ProcessHttpResponse<T>(HttpResponseMessage response, HttpStatusCode[] successCodes = null)
        {
            if (successCodes == null || successCodes.Length < 1)
            {
                successCodes = new[] { HttpStatusCode.OK };
            }


            if (typeof(T) == typeof(byte[]) &&
                successCodes.Contains(response.StatusCode)
                )
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                return (T)(object)data;
            }
            else
            {
                // process it as string
                var responseStr = await response.Content.ReadAsStringAsync();
                if (successCodes.Contains(response.StatusCode))
                {
                    if (typeof(T) == typeof(string))
                    {
                        // try and convert it
                        try
                        {
                            return jsonDeserialize<T>(responseStr);
                        }
                        catch (Exception ex)
                        {
                            // odd trick we have to do to get it to return string when we know it's string and T is string
                            return (T)Convert.ChangeType(responseStr, typeof(T));
                        }

                    }
                    else
                    {
                        T result = jsonDeserialize<T>(responseStr);
                        return result;
                    }


                }
                else
                {
                    throw new HttpException(response.StatusCode, responseStr);
                }
            }


        }


    }
}
