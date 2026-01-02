using Nop.Core;

namespace Nop.Plugin.Shipping.SteadFast.Domain;

/// <summary>
/// Represents a SteadFast shipment event log
/// </summary>
public class SteadFastShipmentEventLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the shipment identifier
    /// </summary>
    public int ShipmentId { get; set; }

    /// <summary>
    /// Gets or sets the consignment ID from SteadFast
    /// </summary>
    public string ConsignmentId { get; set; }

    /// <summary>
    /// Gets or sets the event type
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// Gets or sets the old status
    /// </summary>
    public string OldStatus { get; set; }

    /// <summary>
    /// Gets or sets the new status
    /// </summary>
    public string NewStatus { get; set; }

    /// <summary>
    /// Gets or sets the status message
    /// </summary>
    public string StatusMessage { get; set; }

    /// <summary>
    /// Gets or sets the webhook payload
    /// </summary>
    public string WebhookPayload { get; set; }

    /// <summary>
    /// Gets or sets the entity creation date
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }
}
