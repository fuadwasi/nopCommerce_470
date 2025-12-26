namespace Nop.Plugin.Shipping.SteadFast.Models.Api;

/// <summary>
/// Represents a create order request model
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// Gets or sets the invoice number
    /// </summary>
    public string Invoice { get; set; }

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
}

/// <summary>
/// Represents a create order response model
/// </summary>
public class CreateOrderResponse
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
    /// Gets or sets the consignment data
    /// </summary>
    public ConsignmentData Consignment { get; set; }

    /// <summary>
    /// Gets or sets the errors
    /// </summary>
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
    public string ConsignmentId { get; set; }

    /// <summary>
    /// Gets or sets the tracking code
    /// </summary>
    public string TrackingCode { get; set; }

    /// <summary>
    /// Gets or sets the invoice
    /// </summary>
    public string Invoice { get; set; }

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
    /// Gets or sets the status
    /// </summary>
    public string Status { get; set; }
}
