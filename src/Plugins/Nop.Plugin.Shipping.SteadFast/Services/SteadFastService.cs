using Nop.Core;
using Nop.Plugin.Shipping.SteadFast.Domain;
using Nop.Plugin.Shipping.SteadFast.Models.Api;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents SteadFast service implementation (facade)
/// </summary>
public class SteadFastService : ISteadFastService
{
    #region Fields

    private readonly ISteadFastApiClient _apiClient;
    private readonly ISteadFastShipmentRecordService _shipmentRecordService;

    #endregion

    #region Ctor

    public SteadFastService(
        ISteadFastApiClient apiClient,
        ISteadFastShipmentRecordService shipmentRecordService)
    {
        _apiClient = apiClient;
        _shipmentRecordService = shipmentRecordService;
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
        return await _apiClient.CreateOrderAsync(request);
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
        return await _apiClient.GetBalanceAsync();
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
        return await _apiClient.GetStatusByConsignmentIdAsync(consignmentId);
    }

    /// <summary>
    /// Insert a SteadFast shipment record
    /// </summary>
    /// <param name="shipmentRecord">Shipment record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertShipmentRecordAsync(SteadFastShipmentRecord shipmentRecord)
    {
        await _shipmentRecordService.InsertShipmentRecordAsync(shipmentRecord);
    }

    /// <summary>
    /// Update a SteadFast shipment record
    /// </summary>
    /// <param name="shipmentRecord">Shipment record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateShipmentRecordAsync(SteadFastShipmentRecord shipmentRecord)
    {
        await _shipmentRecordService.UpdateShipmentRecordAsync(shipmentRecord);
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
        return await _shipmentRecordService.GetShipmentRecordByShipmentIdAsync(shipmentId);
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
        return await _shipmentRecordService.GetShipmentRecordByOrderIdAsync(orderId);
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
        return await _shipmentRecordService.GetAllShipmentRecordsAsync(orderId, consignmentId, pageIndex, pageSize);
    }

    #endregion
}
