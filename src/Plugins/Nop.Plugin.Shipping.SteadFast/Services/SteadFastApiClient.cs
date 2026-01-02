using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
    //protected virtual HttpClient PrepareHttpClient()
    //{
    //    if(string.IsNullOrEmpty(_steadFastSettings.ApiKey) ||
    //       string.IsNullOrEmpty(_steadFastSettings.ApiSecretKey))
    //        throw new InvalidOperationException("SteadFast API credentials are not configured.");

    //    return SteadFastApiClientHelper.GetHttpClient(_httpClientFactory, _steadFastSettings.ApiKey, _steadFastSettings.ApiSecretKey);
    //}

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
    public virtual async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest requestData)
    {
        ArgumentNullException.ThrowIfNull(requestData);


        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(SteadFastDefaults.API_BASE_URL, "create_order"));
        request.Headers.Add("Api-Key", _steadFastSettings.ApiKey);
        request.Headers.Add("Secret-Key", _steadFastSettings.ApiSecretKey);
        var content = new StringContent(
            JsonSerializer.Serialize(requestData),
            Encoding.UTF8,
            "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
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
        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(SteadFastDefaults.API_BASE_URL, "get_balance"));
        request.Headers.Add("Api-Key", _steadFastSettings.ApiKey);
        request.Headers.Add("Secret-Key", _steadFastSettings.ApiSecretKey);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
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

        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(SteadFastDefaults.API_BASE_URL, $"status_by_cid/{consignmentId}"));
        request.Headers.Add("Api-Key", _steadFastSettings.ApiKey);
        request.Headers.Add("Secret-Key", _steadFastSettings.ApiSecretKey);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<StatusByConsignmentIdResponse>(responseContent, options);

        return result ?? new StatusByConsignmentIdResponse { Status = 500, Message = "Failed to parse response" };
    }
    /// <summary>
    /// Get status by tracking code
    /// </summary>
    /// <param name="trackingCode">Tracking code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the status response
    /// </returns>
    public virtual async Task<StatusByTrackingCodeResponse> GetStatusByTrackingCodeAsync(string trackingCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(trackingCode);
        var client = new HttpClient();

        var request = new HttpRequestMessage(HttpMethod.Get, string.Format(SteadFastDefaults.API_BASE_URL, $"status_by_trackingcode/{trackingCode}"));
        request.Headers.Add("Api-Key", _steadFastSettings.ApiKey);
        request.Headers.Add("Secret-Key", _steadFastSettings.ApiSecretKey);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<StatusByTrackingCodeResponse>(responseContent, options);
        return result ?? new StatusByTrackingCodeResponse { Status = 500, DeliveryStatus = "Failed to parse response" };
    }

    #endregion
}
