using Nop.Core;
using Nop.Data;
using Nop.Plugin.Shipping.SteadFast.Domain;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents SteadFast shipment event log service implementation
/// </summary>
public class SteadFastShipmentEventLogService : ISteadFastShipmentEventLogService
{
    #region Fields

    private readonly IRepository<SteadFastShipmentEventLog> _eventLogRepository;

    #endregion

    #region Ctor

    public SteadFastShipmentEventLogService(IRepository<SteadFastShipmentEventLog> eventLogRepository)
    {
        _eventLogRepository = eventLogRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Insert a shipment event log
    /// </summary>
    /// <param name="eventLog">Event log</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertEventLogAsync(SteadFastShipmentEventLog eventLog)
    {
        ArgumentNullException.ThrowIfNull(eventLog);

        await _eventLogRepository.InsertAsync(eventLog);
    }

    /// <summary>
    /// Get event logs by shipment ID
    /// </summary>
    /// <param name="shipmentId">Shipment ID</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the event logs
    /// </returns>
    public virtual async Task<IPagedList<SteadFastShipmentEventLog>> GetEventLogsByShipmentIdAsync(
        int shipmentId,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
        if (shipmentId == 0)
            return new PagedList<SteadFastShipmentEventLog>(new List<SteadFastShipmentEventLog>(), pageIndex, pageSize);

        var query = _eventLogRepository.Table
            .Where(x => x.ShipmentId == shipmentId)
            .OrderByDescending(x => x.CreatedOnUtc);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    /// <summary>
    /// Get event logs by consignment ID
    /// </summary>
    /// <param name="consignmentId">Consignment ID</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the event logs
    /// </returns>
    public virtual async Task<IPagedList<SteadFastShipmentEventLog>> GetEventLogsByConsignmentIdAsync(
        string consignmentId,
        int pageIndex = 0,
        int pageSize = int.MaxValue)
    {
        if (string.IsNullOrEmpty(consignmentId))
            return new PagedList<SteadFastShipmentEventLog>(new List<SteadFastShipmentEventLog>(), pageIndex, pageSize);

        var query = _eventLogRepository.Table
            .Where(x => x.ConsignmentId == consignmentId)
            .OrderByDescending(x => x.CreatedOnUtc);

        return await query.ToPagedListAsync(pageIndex, pageSize);
    }

    #endregion
}
