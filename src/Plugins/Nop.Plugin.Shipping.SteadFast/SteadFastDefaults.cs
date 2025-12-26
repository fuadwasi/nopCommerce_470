namespace Nop.Plugin.Shipping.SteadFast;

/// <summary>
/// Represents plugin constants
/// </summary>
public class SteadFastDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public static string SystemName => "Shipping.SteadFast";

    /// <summary>
    /// Gets the configuration route name
    /// </summary>
    public static string ConfigurationRouteName => "Plugin.Shipping.SteadFast.Configure";

    /// <summary>
    /// Gets the shipment list route name
    /// </summary>
    public static string ShipmentListRouteName => "Plugin.Shipping.SteadFast.ShipmentList";

    /// <summary>
    /// Gets the webhook route name
    /// </summary>
    public static string WebhookRouteName => "Plugin.Shipping.SteadFast.Webhook";

    /// <summary>
    /// Gets the webhook path
    /// </summary>
    public const string WEBHOOK_PATH = "Plugins/SteadFast/Webhook";

    /// <summary>
    /// Gets the fixed rate settings key
    /// </summary>
    public const string FIXED_RATE_SETTINGS_KEY = "ShippingRateComputationMethod.FixedOrByWeight.Rate.ShippingMethodId{0}";

    /// <summary>
    /// Gets the transit days settings key
    /// </summary>
    public const string TRANSIT_DAYS_SETTINGS_KEY = "ShippingRateComputationMethod.FixedOrByWeight.TransitDays.ShippingMethodId{0}";

    /// <summary>
    /// SteadFast API base URL
    /// </summary>
    public const string API_BASE_URL = "https://portal.packzy.com/api/v1";
}
