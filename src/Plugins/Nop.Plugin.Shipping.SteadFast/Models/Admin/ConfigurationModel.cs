using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.SteadFast.Models.Admin;

/// <summary>
/// Represents a configuration model
/// </summary>
public record ConfigurationModel : BaseSearchModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.ApiKey")]
    public string ApiKey { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.ApiSecretKey")]
    public string ApiSecretKey { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.UseSandbox")]
    public bool UseSandbox { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.AutoCreateShipment")]
    public bool AutoCreateShipment { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.DefaultNote")]
    public string DefaultNote { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.ShippingByWeightByTotalEnabled")]
    public bool ShippingByWeightByTotalEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.LimitMethodsToCreated")]
    public bool LimitMethodsToCreated { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.WebhookEnabled")]
    public bool WebhookEnabled { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.WebhookSecret")]
    public string WebhookSecret { get; set; }

    public string WebhookUrl { get; set; }

    public decimal? CurrentBalance { get; set; }

    // Search filters for rate by weight by total
    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.Store")]
    public int SearchStoreId { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.Warehouse")]
    public int SearchWarehouseId { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.Country")]
    public int SearchCountryId { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.StateProvince")]
    public int SearchStateProvinceId { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.Zip")]
    public string SearchZip { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Fields.ShippingMethod")]
    public int SearchShippingMethodId { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableStates { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableShippingMethods { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableStores { get; set; } = new List<SelectListItem>();
    public IList<SelectListItem> AvailableWarehouses { get; set; } = new List<SelectListItem>();

    #endregion
}
