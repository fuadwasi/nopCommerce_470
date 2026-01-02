using Nop.Core;

namespace Nop.Plugin.Shipping.SteadFast.Domain;

/// <summary>
/// Represents a SteadFast shipment record
/// </summary>
public class SteadFastShipmentRecord : BaseEntity
{
    /// <summary>
    /// Gets or sets the shipment identifier
    /// </summary>
    public int ShipmentId { get; set; }

    /// <summary>
    /// Gets or sets the order identifier
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the consignment ID from SteadFast
    /// </summary>
    public string ConsignmentId { get; set; }

    /// <summary>
    /// Gets or sets the invoice number
    /// </summary>
    public string InvoiceNumber { get; set; }

    /// <summary>
    /// Gets or sets the recipient name
    /// </summary>
    public string RecipientName { get; set; }

    /// <summary>
    /// Gets or sets the recipient phone
    /// </summary>
    public string RecipientPhone { get; set; }

    /// <summary>
    /// Gets or sets the recipient address
    /// </summary>
    public string RecipientAddress { get; set; }

    /// <summary>
    /// Gets or sets the COD amount
    /// </summary>
    public decimal CodAmount { get; set; }

    /// <summary>
    /// Gets or sets the note
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// Gets or sets the delivery status
    /// </summary>
    public string DeliveryStatus { get; set; }

    /// <summary>
    /// Gets or sets the tracking number
    /// </summary>
    public string TrackingNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the shipment was sent successfully
    /// </summary>
    public bool IsSent { get; set; }

    /// <summary>
    /// Gets or sets the API response
    /// </summary>
    public string ApiResponse { get; set; }

    /// <summary>
    /// Gets or sets the entity creation date
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the entity update date
    /// </summary>
    public DateTime? UpdatedOnUtc { get; set; }
}
