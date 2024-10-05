using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace nac.http;


public class HttpClient
{

    private System.Net.Http.HttpClient http;


    // compatibility methods
    //  !! VERY IMPORTANT !!
    //  You cannot keep adding optional args, that won't work because programs compile to a method signature and if you add optional args you can't bind to that old method signature
    //  see: https://stackoverflow.com/questions/9884664/system-missingmethodexception-after-adding-an-optional-parameter

    public HttpClient(string baseUrl="", bool useWindowsAuth = true, TimeSpan? timeout = null) :
        this(baseUrl: baseUrl,
            args: new model.HttpClientConfigurationOptions
            {
                useWindowsAuth = useWindowsAuth,
                timeout = timeout
            })
    { }

    // OLD METHOD:  This is the redone old method to be based on an argument block so it's runtime decisions
    public HttpClient(string baseUrl, model.HttpClientConfigurationOptions args)
    {
        var baseHandler = new System.Net.Http.HttpClientHandler();

        if (args.useWindowsAuth)
        {
            baseHandler.UseDefaultCredentials = true;
        }

        var curlLoggingHandler = new nac.http.logging.curl.LoggingHandler(baseHandler);
        var harLoggingHandler = new nac.http.logging.har.LoggingHandler(curlLoggingHandler);

        this.http = new System.Net.Http.HttpClient(harLoggingHandler);

        if (args.timeout.HasValue)
        {
            http.Timeout = args.timeout.Value;
        }

        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            http.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
        }
        
        http.DefaultRequestHeaders.Accept.Clear();

        if (args.useBearerTokenAuthentication)
        {
            if (args.useWindowsAuth)
            {
                throw new Exception("You must turn windows authentication off to use bearer authentication");
            }
            if (string.IsNullOrWhiteSpace(args.bearerToken))
            {
                throw new Exception($"You must specify a bearer token to use bearer token authentication");
            }
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", args.bearerToken);
        }

        if (args.BasicAuthentication != null)
        {
            if (string.IsNullOrWhiteSpace(args.BasicAuthentication.username) ||
                string.IsNullOrWhiteSpace(args.BasicAuthentication.password)
               )
            {
                throw new Exception(
                    "You cannot use basic authentication if you don't pass both username, and password.  One or both of them is blank");
            }

            // prepare the basic auth header
            var authenticationString = $"{args.BasicAuthentication.username}:{args.BasicAuthentication.password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(authenticationString));
            http.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
        }

        if (args.headers != null)
        {
            foreach (var pair in args.headers)
            {
                http.DefaultRequestHeaders.Add(pair.Key, pair.Value);
            }
        }


        http.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
    }


    public HttpClient AddDefaultRequestHeader(string key, string value)
    {
        http.DefaultRequestHeaders.Add(key, value);
        return this;
    }

    public HttpClient SetDefaultRequestHeader(string key, string value)
    {
        // Headers should be case insensitive see: RFC2616: (4.2 Message Headers)
        //   + https://stackoverflow.com/questions/11616964/is-request-headersheader-name-in-asp-net-case-sensitive
        if (http.DefaultRequestHeaders.Contains(key))
        {
            http.DefaultRequestHeaders.Remove(key);
        }

        http.DefaultRequestHeaders.Add(key, value);
        return this;
    }


    public async Task<T> PostJSONAsync<T>(object obj, HttpStatusCode[] successCodes = null)
    {
        return await PostJSONAsync<T>("", obj, successCodes);
    }


    public async Task<T> PostJSONAsync<T>(string relativeUrl, Dictionary<string, string> queryParameters,
        object obj, HttpStatusCode[] successCodes = null)
    {
        var url = formBaseRelativeUrlWithQueryParameters(relativeUrl, queryParameters.ToList());

        return await PostJSONAsync<T>(relativeUrl: url, obj: obj, successCodes: successCodes);
    }



    public async Task<T> PostJSONAsync<T>(string relativeUrl, object obj, HttpStatusCode[] successCodes = null)
    {
        var jsonStr = nac.json.utility.SerializeToJSON(obj);
        var jsonStrContent = new StringContent(jsonStr, Encoding.UTF8, "application/json");

        var response = await this.http.PostAsync(relativeUrl, jsonStrContent);

        return await ProcessHttpResponse<T>(response, successCodes: successCodes);
    }


    public async Task<T> PutJSONAsync<T>(object obj, HttpStatusCode[] successCodes = null)
    {
        return await PutJSONAsync<T>(relativeUrl: "", obj: obj, successCodes: successCodes);
    }

    public async Task<T> PutJSONAsync<T>(string relativeUrl, object obj, HttpStatusCode[] successCodes = null)
    {
        var jsonStr = nac.json.utility.SerializeToJSON(obj);
        var jsonStrContent = new StringContent(jsonStr, Encoding.UTF8, "application/json");

        var response = await this.http.PutAsync(relativeUrl, jsonStrContent);

        return await ProcessHttpResponse<T>(response, successCodes: successCodes);
    }

    public async Task<T> PutStreamAsync<T>(string relativeUrl, System.IO.Stream stream, string streamMediaType, HttpStatusCode[] statusCodes = null)
    {
        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(streamMediaType);

        var response = await this.http.PutAsync(relativeUrl, content);

        return await ProcessHttpResponse<T>(response, successCodes: statusCodes);
    }

    public async Task<T> PostFormUrlEncodeAsync<T>(string relativeUrl, IEnumerable<KeyValuePair<string, string>> values, HttpStatusCode[] successCodes = null)
    {
        HttpContent formContent = new FormUrlEncodedContent(values);

        var response = await this.http.PostAsync(relativeUrl, formContent);
        return await ProcessHttpResponse<T>(response, successCodes: successCodes);
    }


    public async Task PostBinary(string relativeUrl, Dictionary<string, string> queryParameters, System.IO.Stream stream)
    {
        await PostBinary<string>(relativeUrl, queryParameters, stream);
    }

    public async Task PostBinary(string relativeUrl, Dictionary<string, string> queryParameters, byte[] binData)
    {
        await PostBinary<string>(relativeUrl, queryParameters, binData);
    }

    public async Task<T> PostBinary<T>(string relativeUrl, Dictionary<string, string> queryParameters, byte[] binData)
    {
        using (var ms = new System.IO.MemoryStream(binData))
        {
            T result = await PostBinary<T>(relativeUrl, queryParameters, ms);
            return result;
        }
    }


    public async Task<T> PostBinary<T>(string relativeUrl, Dictionary<string, string> queryParameters,
        System.IO.Stream stream)
    {
        var url = formBaseRelativeUrlWithQueryParameters(relativeUrl, queryParameters.ToList());

        var content = new StreamContent(stream);
        var response = await this.http.PostAsync(url, content);
        T result = await ProcessHttpResponse<T>(response);

        return result;
    }


    public async Task<T> PostText<T>(string relativeUrl,
                                    string content,
                                    string contentType = "text/plain",
                                    HttpStatusCode[] successCodes = null)
    {
        var strContent = new StringContent(content: content, encoding: Encoding.UTF8, mediaType: contentType);

        var response = await this.http.PostAsync(relativeUrl, strContent);
        return await ProcessHttpResponse<T>(response, successCodes: successCodes);
    }


    public async Task<T> PutFormUrlEncodeAsync<T>(string relativeUrl, IEnumerable<KeyValuePair<string, string>> values, HttpStatusCode[] successCodes = null)
    {
        HttpContent formContent = new FormUrlEncodedContent(values);

        var response = await this.http.PutAsync(relativeUrl, formContent);
        return await ProcessHttpResponse<T>(response, successCodes: successCodes);
    }


    public async Task<T> DeleteAsync<T>(string relativeUrl, HttpStatusCode[] successCodes = null)
    {
        var response = await this.http.DeleteAsync(relativeUrl);
        return await ProcessHttpResponse<T>(response, successCodes: successCodes);
    }

    public async Task<T> DeleteAsync<T>(string relativeUrl, Dictionary<string, string> queryParameters, HttpStatusCode[] successCodes = null)
    {
        var url = formBaseRelativeUrlWithQueryParameters(relativeUrl, queryParameters.ToList());

        return await DeleteAsync<T>(url, successCodes: successCodes);
    }

    /**
     <summary>
        If there are no parameters either on relativeUrl and queryParameters is null it will return a url without parameters on it
     </summary>
     */
    private static string formBaseRelativeUrlWithQueryParameters(string relativeUrl, List<KeyValuePair<string, string>> queryParameters)
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

        // returns an implementation of NameValueCollection
        // which in fact is HttpValueCollection
        var values = System.Web.HttpUtility.ParseQueryString(baseQuery);
        if (queryParameters != null)
        {
            foreach (var pair in queryParameters)
            {
                values.Add(pair.Key, pair.Value);
            }
        }

        if (values.Count > 0)
        {
            return baseRelativeUrl + '?' + values.ToString();
        }
        else
        {
            return baseRelativeUrl;
        }

    }


    public async Task<T> GetJSONAsync<T>(string relativeUrl, List<KeyValuePair<string, string>> queryParameters)
    {
        var url = formBaseRelativeUrlWithQueryParameters(relativeUrl, queryParameters);

        return await GetJSONAsync<T>(url);
    }


    public async Task<T> GetJSONAsync<T>(string relativeUrl, Dictionary<string, string> queryParameters)
    {
        var url = formBaseRelativeUrlWithQueryParameters(relativeUrl, queryParameters.ToList());

        return await GetJSONAsync<T>(url);
    }


    public async Task<T> GetJSONAsync<T>(string relativeUrl)
    {
        var response = await this.http.GetAsync(relativeUrl);

        return await ProcessHttpResponse<T>(response);
    }


    public async Task<T> GetJSONAsync<T>(string relativeUrl, Dictionary<string, string> queryParameters, object bodyData)
    {
        var jsonStr = nac.json.utility.SerializeToJSON(bodyData);
        var jsonStrContent = new StringContent(jsonStr, Encoding.UTF8, "application/json");

        var url = formBaseRelativeUrlWithQueryParameters(relativeUrl, queryParameters.ToList());

        var requestUrl = new Uri(http.BaseAddress, url);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = requestUrl,
            Content = jsonStrContent,
        };

        var response = await this.http.SendAsync(request);

        return await ProcessHttpResponse<T>(response);
    }


    protected virtual async Task<T> ProcessHttpResponse<T>(HttpResponseMessage response, HttpStatusCode[] successCodes = null)
    {
        if (successCodes == null || successCodes.Length < 1)
        {
            successCodes = new[] { HttpStatusCode.OK, HttpStatusCode.NoContent };
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType("", typeof(T)); // have to do this even though we know T is string
            }
            else
            {
                return default(T);
            }
        }


        if (typeof(T) == typeof(byte[]) &&
            successCodes.Contains(response.StatusCode)
            )
        {
            var data = await response.Content.ReadAsByteArrayAsync();
            return (T)(object)data;
        }


        // process it as string
        var responseStr = await response.Content.ReadAsStringAsync();

        if (!successCodes.Contains(response.StatusCode))
        {
            throw new HttpException(response.StatusCode, responseStr);
        }

        if (typeof(T) == typeof(string))
        {
            // try and convert it
            try
            {
                return nac.json.utility.DeserializeJSON<T>(responseStr);
            }
            catch (Exception ex)
            {
                // odd trick we have to do to get it to return string when we know it's string and T is string
                return (T)Convert.ChangeType(responseStr, typeof(T));
            }

        }

        if (typeof(T) == typeof(System.Xml.Linq.XDocument))
        {
            // interpret the response body as XML
            var xmlResult = System.Xml.Linq.XDocument.Parse(responseStr);
            return (T)Convert.ChangeType(xmlResult, typeof(T));
        }

        /*
         This will cover these types
             Dictionary<string,object>
             Dictionary<string,string>
             List<Dictionary<string,object>>
         */
        if (typeof(T).FullName.Contains("System.Collections.Generic.Dictionary")
            )
        {
            var obj = nac.json.utility.DeserializeToDictionaryList(responseStr, nac.json.model.ObjectFormat.Dictionary);
            if (obj is List<object> listOfObjects)
            {
                return ChangeTypeListObjectToListDictionary<T>(listOfObjects);
            }

            return (T)obj;
        }

        if (typeof(T) == typeof(object) ||
            typeof(T) == typeof(List<object>)
            )
        {
            var obj = nac.json.utility.DeserializeToDictionaryList(responseStr, nac.json.model.ObjectFormat.Flexpando);

            return (T)obj;
        }

        T result = nac.json.utility.DeserializeJSON<T>(responseStr);
        return result;




    }



    private T ChangeTypeListObjectToListDictionary<T>(List<object> listOfObjects)
    {
        var dictList = new List<Dictionary<string, object>>();

        foreach (var i in listOfObjects)
        {
            dictList.Add(i as Dictionary<string, object>);
        }

        return (T)Convert.ChangeType(dictList, typeof(T));
    }



}




