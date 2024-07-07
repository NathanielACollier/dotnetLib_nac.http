using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nac.http.model;

public class HttpClientConfigurationOptions
{
    public bool useWindowsAuth { get; set; } = true; // default to use windows auth by default for compaitibility
    public TimeSpan? timeout { get; set; } = null;
    public bool useBearerTokenAuthentication { get; set; } = false;
    public string bearerToken { get; set; } = null;

    public model.BasicAuthenticationOptions BasicAuthentication { get; set; } = null;

    public Dictionary<string, string> headers { get; set; } = null; // these will be passed on every call

    public HttpClientConfigurationOptions()
    {

    }

}
