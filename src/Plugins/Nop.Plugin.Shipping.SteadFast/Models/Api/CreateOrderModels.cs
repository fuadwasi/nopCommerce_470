using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.SteadFast.Models.Api;

/// <summary>
/// Represents a create order request model
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// Gets or sets the invoice number
    /// </summary>
    [JsonProperty(PropertyName = "invoice")]
    public string Invoice { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipient name
    /// </summary>
    [JsonProperty(PropertyName = "recipient_name")]
    public string RecipientName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipient phone
    /// </summary>
    [JsonProperty(PropertyName = "recipient_phone")]
    public string RecipientPhone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the alternative phone
    /// </summary>
    [JsonProperty(PropertyName = "alternative_phone")]
    public string? AlternativePhone { get; set; } = null;

    /// <summary>
    /// Gets or sets the recipient email
    /// </summary>
    [JsonProperty(PropertyName = "recipient_email")]
    public string? RecipientEmail { get; set; } = null;

    /// <summary>
    /// Gets or sets the recipient address
    /// </summary>
    [JsonProperty(PropertyName = "recipient_address")]
    public string RecipientAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the COD amount
    /// </summary>
    [JsonProperty(PropertyName = "cod_amount")]
    public decimal CodAmount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the note
    /// </summary>
    [JsonProperty(PropertyName = "note")]
    public string? Note { get; set; } = null;

    /// <summary>
    /// Gets or sets the item description
    /// </summary>
    [JsonProperty(PropertyName = "item_description")]
    public string? ItemDescription { get; set; } = null;

    /// <summary>
    /// Gets or sets the total lot
    /// </summary>
    [JsonProperty(PropertyName = "total_lot")]
    public int? TotalLot { get; set; } = null;

    /// <summary>
    /// Gets or sets the delivery type
    /// </summary>
    [JsonProperty(PropertyName = "delivery_type")]
    public int? DeliveryType { get; set; } = null; // 0 = home delivery, 1 = hub pickup
}

/// <summary>
/// Represents a create order response model
/// </summary>
public class CreateOrderResponse
{
    /// <summary>
    /// Gets or sets the status code
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the message
    /// </summary>
    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the consignment data
    /// </summary>
    [JsonProperty(PropertyName = "consignment")]
    public ConsignmentData Consignment { get; set; }

    /// <summary>
    /// Gets or sets the errors
    /// </summary>
    [JsonProperty(PropertyName = "errors")]
    public Dictionary<string, List<string>> Errors { get; set; }
}

/// <summary>
/// Represents consignment data
/// </summary>
public class ConsignmentData
{
    /// <summary>
    /// Gets or sets the consignment ID
    /// </summary>
    [JsonProperty(PropertyName = "consignment_id")]
    public string ConsignmentId { get; set; }

    /// <summary>
    /// Gets or sets the tracking code
    /// </summary>
    [JsonProperty(PropertyName = "tracking_code")]
    public string TrackingCode { get; set; }

    /// <summary>
    /// Gets or sets the invoice
    /// </summary>
    [JsonProperty(PropertyName = "invoice")]
    public string Invoice { get; set; }

    /// <summary>
    /// Gets or sets the recipient name
    /// </summary>
    [JsonProperty(PropertyName = "recipient_name")]
    public string RecipientName { get; set; }

    /// <summary>
    /// Gets or sets the recipient phone
    /// </summary>
    [JsonProperty(PropertyName = "recipient_phone")]
    public string RecipientPhone { get; set; }

    /// <summary>
    /// Gets or sets the recipient address
    /// </summary>
    [JsonProperty(PropertyName = "recipient_address")]
    public string RecipientAddress { get; set; }

    /// <summary>
    /// Gets or sets the COD amount
    /// </summary>
    [JsonProperty(PropertyName = "cod_amount")]
    public decimal CodAmount { get; set; }

    /// <summary>
    /// Gets or sets the status
    /// </summary>
    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }
}
