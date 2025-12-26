using Nop.Web.Framework.Models;

namespace Nop.Plugin.Shipping.SteadFast.Models.Api;

/// <summary>
/// Represents a get balance response model
/// </summary>
public class GetBalanceResponse : BaseNopModel
{
    /// <summary>
    /// Gets or sets the status code
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the current balance
    /// </summary>
    public decimal CurrentBalance { get; set; }
}

/// <summary>
/// Represents a status by consignment ID response model
/// </summary>
public class StatusByConsignmentIdResponse : BaseNopModel
{
    /// <summary>
    /// Gets or sets the status code
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Gets or sets the message
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the delivery status
    /// </summary>
    public string DeliveryStatus { get; set; }

    /// <summary>
    /// Gets or sets the consignment ID
    /// </summary>
    public string ConsignmentId { get; set; }

    /// <summary>
    /// Gets or sets the tracking code
    /// </summary>
    public string TrackingCode { get; set; }

    /// <summary>
    /// Gets or sets additional status information
    /// </summary>
    public Dictionary<string, object> Data { get; set; }
}
