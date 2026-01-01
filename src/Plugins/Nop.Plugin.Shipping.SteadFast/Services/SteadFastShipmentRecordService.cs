using Nop.Core;
using Nop.Data;
using Nop.Plugin.Shipping.SteadFast.Domain;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents SteadFast shipment record service implementation
/// </summary>
public class SteadFastShipmentRecordService : ISteadFastShipmentRecordService
{
    #region Fields

    private readonly IRepository<SteadFastShipmentRecord> _shipmentRecordRepository;

    #endregion

    #region Ctor

    public SteadFastShipmentRecordService(IRepository<SteadFastShipmentRecord> shipmentRecordRepository)
    {
        _shipmentRecordRepository = shipmentRecordRepository;
    }

    #endregion

    #region Methods

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
    /// Get a SteadFast shipment record by Tracking Number
    /// </summary>
    /// <param name="trackingNumber">Tracking Number</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment record
    /// </returns>
    public virtual async Task<SteadFastShipmentRecord> GetShipmentRecordByTrackingNumberAsync(string trackingNumber)
    {
        if (string.IsNullOrEmpty(trackingNumber))
            return null;

        return await _shipmentRecordRepository.Table
            .FirstOrDefaultAsync(x => x.TrackingNumber == trackingNumber);
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
