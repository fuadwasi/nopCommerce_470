using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Plugin.Shipping.SteadFast.Services;

namespace Nop.Plugin.Shipping.SteadFast.Tracking;

/// <summary>
/// SteadFast shipment tracker implementation
/// </summary>
public class SteadFastShipmentTracker : IShipmentTracker
{
    private readonly ISteadFastShipmentRecordService _shipmentRecordService;
    private readonly ISteadFastShipmentEventLogService _eventLogService;

    public SteadFastShipmentTracker(ISteadFastShipmentRecordService shipmentRecordService,
        ISteadFastShipmentEventLogService eventLogService)
    {
        _shipmentRecordService = shipmentRecordService;
        _eventLogService = eventLogService;
    }

    public Task<string> GetUrlAsync(string trackingNumber, Shipment shipment = null)
    {
        // SteadFast tracking URL format
        var url = $"https://steadfast.com.bd/t/{trackingNumber}";
        return Task.FromResult(url);
    }

    public async Task<IList<ShipmentStatusEvent>> GetShipmentEventsAsync(string trackingNumber, Shipment shipment = null)
    {
        var result = new List<ShipmentStatusEvent>();
        if (string.IsNullOrEmpty(trackingNumber))
            return result;

        var shipmentRecord = await _shipmentRecordService.GetShipmentRecordByTrackingNumberAsync(trackingNumber);
        if(shipmentRecord == null)
            return result;

        // Get all event logs for this tracking number
        var eventLogs = await _eventLogService.GetEventLogsByShipmentIdAsync(shipmentRecord.ShipmentId);
        foreach (var log in eventLogs)
        {
            result.Add(new ShipmentStatusEvent
            {
                EventName = log.EventType,
                Status = log.NewStatus,
                Location = null,
                CountryCode = null,
                Date = log.CreatedOnUtc
            });
        }
        return result;
    }
}
