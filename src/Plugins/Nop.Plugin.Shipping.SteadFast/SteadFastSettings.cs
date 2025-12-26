using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.SteadFast;

/// <summary>
/// Represents settings of the SteadFast shipping plugin
/// </summary>
public class SteadFastSettings : ISettings
{
    /// <summary>
    /// Gets or sets API Key
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets API Secret Key
    /// </summary>
    public string ApiSecretKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use sandbox (test environment)
    /// </summary>
    public bool UseSandbox { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the shipping by weight and by total is enabled
    /// </summary>
    public bool ShippingByWeightByTotalEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to limit shipping methods to configured ones
    /// </summary>
    public bool LimitMethodsToCreated { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to load all records from database
    /// </summary>
    public bool LoadAllRecord { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to automatically create shipment on SteadFast
    /// </summary>
    public bool AutoCreateShipment { get; set; }

    /// <summary>
    /// Gets or sets default note for shipments
    /// </summary>
    public string DefaultNote { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether webhook is enabled
    /// </summary>
    public bool WebhookEnabled { get; set; }

    /// <summary>
    /// Gets or sets the webhook secret for verification
    /// </summary>
    public string WebhookSecret { get; set; }
}
