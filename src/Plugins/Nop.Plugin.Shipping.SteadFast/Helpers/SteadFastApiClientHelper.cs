//using System.Net.Http.Headers;

//namespace Nop.Plugin.Shipping.SteadFast.Helpers;

//public static class SteadFastApiClientHelper
//{
//    public static HttpClient GetHttpClient(IHttpClientFactory httpClientFactory, string apiKey,  string apiSecretKey)
//    {
//        //var client = httpClientFactory.CreateClient();
//        //client.BaseAddress = new Uri(SteadFastDefaults.API_BASE_URL);
//        //client.DefaultRequestHeaders.Accept.Clear();
//        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//        //client.DefaultRequestHeaders.Add("Api-Key", apiKey);
//        //client.DefaultRequestHeaders.Add("Secret-Key", apiSecretKey);
//        ////client.DefaultRequestHeaders.Add("Content-Type", "application/json");

//        var client = new HttpClient();
//        var request = new HttpRequestMessage(HttpMethod.Get, "https://portal.packzy.com/api/v1/get_balance");
//        request.Headers.Add("Api-Key", apiKey);
//        request.Headers.Add("Secret-Key", apiSecretKey);
//        var response = await client.SendAsync(request);
//        response.EnsureSuccessStatusCode();
//        Console.WriteLine(await response.Content.ReadAsStringAsync());


//        return client;
//    }
//}
