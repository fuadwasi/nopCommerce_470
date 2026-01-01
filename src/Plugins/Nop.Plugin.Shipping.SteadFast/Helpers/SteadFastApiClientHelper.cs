using System.Net.Http.Headers;

namespace Nop.Plugin.Shipping.SteadFast.Helpers;

public static class SteadFastApiClientHelper
{
    public static HttpClient GetHttpClient(IHttpClientFactory httpClientFactory, string apiKey,  string apiSecretKey)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(SteadFastDefaults.API_BASE_URL);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Api-Key", apiKey);
        client.DefaultRequestHeaders.Add("Secret-Key", apiSecretKey);
        client.DefaultRequestHeaders.Add("Content-Type", "application/json");

        return client;
    }
}
