# NC.HttpClient

## Design Ideas
```c#
var client = new NC.HttpClient("http://www.test.com");

var result = await client.GetJSONAsync<string>("api/test", new Dictionary<string,string>{
    {"apiKey","blue whale"},
    {"p1", "Orange Juice"}
})
```