using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.SteadFast.Models.Api;

/// <summary>
/// Represents a get balance response model
/// </summary>
public class GetBalanceResponse
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
    /// Gets or sets the current balance
    /// </summary>
    [JsonProperty(PropertyName = "current_balance")]
    public decimal CurrentBalance { get; set; }
}

/// <summary>
/// Represents a status by consignment ID response model
/// </summary>
public class StatusByConsignmentIdResponse
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
    /// Gets or sets the delivery status
    /// </summary>
    [JsonProperty(PropertyName = "delivery_status")]
    public string DeliveryStatus { get; set; }

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
    /// Gets or sets additional status information
    /// </summary>
    [JsonProperty(PropertyName = "data")]
    public Dictionary<string, object> Data { get; set; }
}
