using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tests_WebApiForTesting.Controllers
{
    [Route("api/[controller]")]
    public class GeneralController : Controller
    {
        // URL will be: https://localhost:5001/api/general/test1
        // The routes build onto the main route listed above
        [HttpGet, Route("test1")]
        public string helloWorld()
        {
            return "Hello World!";
        }



        [HttpPost, Route("testPostHash")]
        public System.Text.Json.Nodes.JsonNode testPostHashMapDictionary([FromBody] System.Text.Json.Nodes.JsonNode args)
        {
            return args;
        }
    }
}
