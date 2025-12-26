using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Shipping.SteadFast.Domain;
using Nop.Plugin.Shipping.SteadFast.Models.Api;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents SteadFast service implementation
/// </summary>
public class SteadFastService : ISteadFastService
{
    #region Fields

    private readonly IRepository<SteadFastShipmentRecord> _shipmentRecordRepository;
    private readonly SteadFastSettings _steadFastSettings;
    private readonly IHttpClientFactory _httpClientFactory;

    #endregion

    #region Ctor

    public SteadFastService(
        IRepository<SteadFastShipmentRecord> shipmentRecordRepository,
        SteadFastSettings steadFastSettings,
        IHttpClientFactory httpClientFactory)
    {
        _shipmentRecordRepository = shipmentRecordRepository;
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

    /// <summary>
    /// Insert a SteadFast shipment record
    /// </summary>
    /// <param name="shipmentRecord">Shipment record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertShipmentRecordAsync(SteadFastShipmentRecord shipmentRecord)
    {
        ArgumentNullException.ThrowIfNull(shipmentRecord);

        await _shipmentRecordRepository.InsertAsync(shipmentRecord);
    }

    /// <summary>
    /// Update a SteadFast shipment record
    /// </summary>
    /// <param name="shipmentRecord">Shipment record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateShipmentRecordAsync(SteadFastShipmentRecord shipmentRecord)
    {
        ArgumentNullException.ThrowIfNull(shipmentRecord);

        await _shipmentRecordRepository.UpdateAsync(shipmentRecord);
    }

    /// <summary>
    /// Get a SteadFast shipment record by shipment ID
    /// </summary>
    /// <param name="shipmentId">Shipment ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment record
    /// </returns>
    public virtual async Task<SteadFastShipmentRecord> GetShipmentRecordByShipmentIdAsync(int shipmentId)
    {
        if (shipmentId == 0)
            return null;

        return await _shipmentRecordRepository.Table
            .FirstOrDefaultAsync(x => x.ShipmentId == shipmentId);
    }

    /// <summary>
    /// Get a SteadFast shipment record by order ID
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment record
    /// </returns>
    public virtual async Task<SteadFastShipmentRecord> GetShipmentRecordByOrderIdAsync(int orderId)
    {
        if (orderId == 0)
            return null;

        return await _shipmentRecordRepository.Table
            .FirstOrDefaultAsync(x => x.OrderId == orderId);
    }

    /// <summary>
    /// Get all SteadFast shipment records
    /// </summary>
    /// <param name="orderId">Order ID; null to load all records</param>
    /// <param name="consignmentId">Consignment ID; null to load all records</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment records
    /// </returns>
    public virtual async Task<IPagedList<SteadFastShipmentRecord>> GetAllShipmentRecordsAsync(
        int? orderId = null,
        string consignmentId = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
        var query = _shipmentRecordRepository.Table;

        if (orderId.HasValue && orderId.Value > 0)
            query = query.Where(x => x.OrderId == orderId.Value);

        if (!string.IsNullOrEmpty(consignmentId))
            query = query.Where(x => x.ConsignmentId == consignmentId);

        query = query.OrderByDescending(x => x.CreatedOnUtc);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion
}
