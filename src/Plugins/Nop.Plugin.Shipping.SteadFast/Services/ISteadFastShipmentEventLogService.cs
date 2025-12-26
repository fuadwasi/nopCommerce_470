using Nop.Core;
using Nop.Plugin.Shipping.SteadFast.Domain;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents SteadFast shipment event log service interface
/// </summary>
public interface ISteadFastShipmentEventLogService
{
    /// <summary>
    /// Insert a shipment event log
    /// </summary>
    /// <param name="eventLog">Event log</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertEventLogAsync(SteadFastShipmentEventLog eventLog);

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
    Task<IPagedList<SteadFastShipmentEventLog>> GetEventLogsByShipmentIdAsync(
        int shipmentId,
        int pageIndex = 0,
        int pageSize = int.MaxValue);

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
    Task<IPagedList<SteadFastShipmentEventLog>> GetEventLogsByConsignmentIdAsync(
        string consignmentId,
        int pageIndex = 0,
        int pageSize = int.MaxValue);
}
