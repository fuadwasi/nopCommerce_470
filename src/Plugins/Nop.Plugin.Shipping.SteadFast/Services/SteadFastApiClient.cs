using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Nop.Plugin.Shipping.SteadFast.Models.Api;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents SteadFast API client implementation
/// </summary>
public class SteadFastApiClient : ISteadFastApiClient
{
    #region Fields

    private readonly SteadFastSettings _steadFastSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    #endregion

    #region Ctor

    public SteadFastApiClient(
        SteadFastSettings steadFastSettings,
        IHttpClientFactory httpClientFactory)
    {
        _steadFastSettings = steadFastSettings;
        _httpClientFactory = httpClientFactory;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare HTTP client with SteadFast API headers
    /// </summary>
    /// <returns>HTTP client</returns>
    protected virtual HttpClient PrepareHttpClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(SteadFastDefaults.API_BASE_URL);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Api-Key", _steadFastSettings.ApiKey);
        client.DefaultRequestHeaders.Add("Secret-Key", _steadFastSettings.ApiSecretKey);
        
        return client;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create order on SteadFast
    /// </summary>
    /// <param name="request">Create order request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the create order response
    /// </returns>
    public virtual async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var client = PrepareHttpClient();

        var requestBody = new
        {
            invoice = request.Invoice,
            recipient_name = request.RecipientName,
            recipient_phone = request.RecipientPhone,
            recipient_address = request.RecipientAddress,
            cod_amount = request.CodAmount,
            note = request.Note
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync("/create_order", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<CreateOrderResponse>(responseContent, options);

        return result ?? new CreateOrderResponse { Status = 500, Message = "Failed to parse response" };
    }

    /// <summary>
    /// Get balance from SteadFast
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the balance response
    /// </returns>
    public virtual async Task<GetBalanceResponse> GetBalanceAsync()
    {
        var client = PrepareHttpClient();

        var response = await client.GetAsync("/get_balance");
        var responseContent = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<GetBalanceResponse>(responseContent, options);

        return result ?? new GetBalanceResponse { Status = 500, Message = "Failed to parse response" };
    }

    /// <summary>
    /// Get status by consignment ID
    /// </summary>
    /// <param name="consignmentId">Consignment ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the status response
    /// </returns>
    public virtual async Task<StatusByConsignmentIdResponse> GetStatusByConsignmentIdAsync(string consignmentId)
    {
        ArgumentException.ThrowIfNullOrEmpty(consignmentId);

        var client = PrepareHttpClient();

        var response = await client.GetAsync($"/status_by_cid/{consignmentId}");
        var responseContent = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<StatusByConsignmentIdResponse>(responseContent, options);

        return result ?? new StatusByConsignmentIdResponse { Status = 500, Message = "Failed to parse response" };
    }

    #endregion
}
