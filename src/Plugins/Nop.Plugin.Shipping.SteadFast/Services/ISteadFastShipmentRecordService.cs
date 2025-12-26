using Nop.Core;
using Nop.Plugin.Shipping.SteadFast.Domain;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents SteadFast shipment record service interface
/// </summary>
public interface ISteadFastShipmentRecordService
{
    /// <summary>
    /// Insert a SteadFast shipment record
    /// </summary>
    /// <param name="shipmentRecord">Shipment record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertShipmentRecordAsync(SteadFastShipmentRecord shipmentRecord);

    /// <summary>
    /// Update a SteadFast shipment record
    /// </summary>
    /// <param name="shipmentRecord">Shipment record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateShipmentRecordAsync(SteadFastShipmentRecord shipmentRecord);

    /// <summary>
    /// Get a SteadFast shipment record by shipment ID
    /// </summary>
    /// <param name="shipmentId">Shipment ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment record
    /// </returns>
    Task<SteadFastShipmentRecord> GetShipmentRecordByShipmentIdAsync(int shipmentId);

    /// <summary>
    /// Get a SteadFast shipment record by order ID
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment record
    /// </returns>
    Task<SteadFastShipmentRecord> GetShipmentRecordByOrderIdAsync(int orderId);

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
    Task<IPagedList<SteadFastShipmentRecord>> GetAllShipmentRecordsAsync(
        int? orderId = null,
        string consignmentId = null,
        int pageIndex = 0,
        int pageSize = int.MaxValue);
}
