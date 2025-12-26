using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.SteadFast.Domain;
using Nop.Plugin.Shipping.SteadFast.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.SteadFast;

/// <summary>
/// SteadFast shipping computation method
/// </summary>
public class SteadFastComputationMethod : BasePlugin, IShippingRateComputationMethod
{
    #region Fields

    private readonly SteadFastSettings _steadFastSettings;
    private readonly ILocalizationService _localizationService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly ISettingService _settingService;
    private readonly IShippingByWeightByTotalService _shippingByWeightByTotalService;
    private readonly IShippingService _shippingService;
    private readonly IStoreContext _storeContext;
    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public SteadFastComputationMethod(
        SteadFastSettings steadFastSettings,
        ILocalizationService localizationService,
        IShoppingCartService shoppingCartService,
        ISettingService settingService,
        IShippingByWeightByTotalService shippingByWeightByTotalService,
        IShippingService shippingService,
        IStoreContext storeContext,
        IWebHelper webHelper)
    {
        _steadFastSettings = steadFastSettings;
        _localizationService = localizationService;
        _shoppingCartService = shoppingCartService;
        _settingService = settingService;
        _shippingByWeightByTotalService = shippingByWeightByTotalService;
        _shippingService = shippingService;
        _storeContext = storeContext;
        _webHelper = webHelper;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get fixed rate
    /// </summary>
    /// <param name="shippingMethodId">Shipping method ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the rate
    /// </returns>
    protected async Task<decimal> GetRateAsync(int shippingMethodId)
    {
        return await _settingService.GetSettingByKeyAsync<decimal>(
            string.Format(SteadFastDefaults.FIXED_RATE_SETTINGS_KEY, shippingMethodId));
    }

    /// <summary>
    /// Gets the transit days
    /// </summary>
    /// <param name="shippingMethodId">Shipping method ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the transit days
    /// </returns>
    protected async Task<int?> GetTransitDaysAsync(int shippingMethodId)
    {
        return await _settingService.GetSettingByKeyAsync<int?>(
            string.Format(SteadFastDefaults.TRANSIT_DAYS_SETTINGS_KEY, shippingMethodId));
    }

    /// <summary>
    /// Get rate by weight and by total
    /// </summary>
    /// <param name="shippingByWeightByTotalRecord">Shipping by weight/by total record</param>
    /// <param name="subTotal">Subtotal</param>
    /// <param name="weight">Weight</param>
    /// <returns>Rate</returns>
    protected decimal GetRate(ShippingByWeightByTotalRecord shippingByWeightByTotalRecord, decimal subTotal, decimal weight)
    {
        //additional fixed cost
        var shippingTotal = shippingByWeightByTotalRecord.AdditionalFixedCost;

        //charge amount per weight unit
        if (shippingByWeightByTotalRecord.RatePerWeightUnit > decimal.Zero)
        {
            var weightRate = Math.Max(weight - shippingByWeightByTotalRecord.LowerWeightLimit, decimal.Zero);
            shippingTotal += shippingByWeightByTotalRecord.RatePerWeightUnit * weightRate;
        }

        //percentage rate of subtotal
        if (shippingByWeightByTotalRecord.PercentageRateOfSubtotal > decimal.Zero)
        {
            shippingTotal += Math.Round((decimal)((((float)subTotal) * 
                ((float)shippingByWeightByTotalRecord.PercentageRateOfSubtotal)) / 100f), 2);
        }

        return Math.Max(shippingTotal, decimal.Zero);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets available shipping options
    /// </summary>
    /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the represents a response of getting shipping rate options
    /// </returns>
    public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
        ArgumentNullException.ThrowIfNull(getShippingOptionRequest);

        var response = new GetShippingOptionResponse();

        if (getShippingOptionRequest.Items == null || !getShippingOptionRequest.Items.Any())
        {
            response.AddError("No shipment items");
            return response;
        }

        //choose the shipping rate calculation method
        if (_steadFastSettings.ShippingByWeightByTotalEnabled)
        {
            //shipping rate calculation by products weight

            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }

            var store = await _storeContext.GetCurrentStoreAsync();
            var storeId = getShippingOptionRequest.StoreId != 0 ? getShippingOptionRequest.StoreId : store.Id;
            var countryId = getShippingOptionRequest.ShippingAddress.CountryId ?? 0;
            var stateProvinceId = getShippingOptionRequest.ShippingAddress.StateProvinceId ?? 0;
            var warehouseId = getShippingOptionRequest.WarehouseFrom?.Id ?? 0;
            var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;

            //get subtotal of shipped items
            var subTotal = decimal.Zero;
            foreach (var packageItem in getShippingOptionRequest.Items)
            {
                if (await _shippingService.IsFreeShippingAsync(packageItem.ShoppingCartItem))
                    continue;

                subTotal += (await _shoppingCartService.GetSubTotalAsync(packageItem.ShoppingCartItem, true)).subTotal;
            }

            //get weight of shipped items (excluding items with free shipping)
            var weight = await _shippingService.GetTotalWeightAsync(getShippingOptionRequest, ignoreFreeShippedItems: true);

            foreach (var shippingMethod in await _shippingService.GetAllShippingMethodsAsync(countryId))
            {
                int? transitDays = null;
                var rate = decimal.Zero;

                var shippingByWeightByTotalRecord = await _shippingByWeightByTotalService.FindRecordsAsync(
                    shippingMethod.Id, storeId, warehouseId, countryId, stateProvinceId, zip, weight, subTotal);
                
                if (shippingByWeightByTotalRecord == null)
                {
                    if (_steadFastSettings.LimitMethodsToCreated)
                        continue;
                }
                else
                {
                    rate = GetRate(shippingByWeightByTotalRecord, subTotal, weight);
                    transitDays = shippingByWeightByTotalRecord.TransitDays;
                }

                response.ShippingOptions.Add(new ShippingOption
                {
                    Name = await _localizationService.GetLocalizedAsync(shippingMethod, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(shippingMethod, x => x.Description),
                    Rate = rate,
                    TransitDays = transitDays
                });
            }
        }
        else
        {
            //shipping rate calculation by fixed rate
            var restrictByCountryId = getShippingOptionRequest.ShippingAddress?.CountryId;
            response.ShippingOptions = await (await _shippingService.GetAllShippingMethodsAsync(restrictByCountryId))
                .SelectAwait(async shippingMethod => new ShippingOption
                {
                    Name = await _localizationService.GetLocalizedAsync(shippingMethod, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(shippingMethod, x => x.Description),
                    Rate = await GetRateAsync(shippingMethod.Id),
                    TransitDays = await GetTransitDaysAsync(shippingMethod.Id)
                }).ToListAsync();
        }

        return response;
    }

    /// <summary>
    /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
    /// </summary>
    /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the fixed shipping rate; or null in case there's no fixed shipping rate
    /// </returns>
    public async Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
    {
        ArgumentNullException.ThrowIfNull(getShippingOptionRequest);

        //if the "shipping calculation by weight" method is selected, the fixed rate isn't calculated
        if (_steadFastSettings.ShippingByWeightByTotalEnabled)
            return null;

        var restrictByCountryId = getShippingOptionRequest.ShippingAddress?.CountryId;
        var rates = await (await _shippingService.GetAllShippingMethodsAsync(restrictByCountryId))
            .SelectAwait(async shippingMethod => await GetRateAsync(shippingMethod.Id))
            .Distinct()
            .ToListAsync();

        //return default rate if all of them equal
        if (rates.Count == 1)
            return rates.FirstOrDefault();

        return null;
    }

    /// <summary>
    /// Get associated shipment tracker
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipment tracker
    /// </returns>
    public Task<IShipmentTracker> GetShipmentTrackerAsync()
    {
        return Task.FromResult<IShipmentTracker>(null);
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/SteadFast/Configure";
    }

    /// <summary>
    /// Install plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //settings
        await _settingService.SaveSettingAsync(new SteadFastSettings
        {
            LoadAllRecord = true,
            AutoCreateShipment = false,
            ShippingByWeightByTotalEnabled = false,
            LimitMethodsToCreated = false
        });

        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            ["Plugins.Shipping.SteadFast.Fields.ApiKey"] = "API Key",
            ["Plugins.Shipping.SteadFast.Fields.ApiKey.Hint"] = "Enter your SteadFast API Key.",
            ["Plugins.Shipping.SteadFast.Fields.ApiSecretKey"] = "API Secret Key",
            ["Plugins.Shipping.SteadFast.Fields.ApiSecretKey.Hint"] = "Enter your SteadFast API Secret Key.",
            ["Plugins.Shipping.SteadFast.Fields.UseSandbox"] = "Use Sandbox",
            ["Plugins.Shipping.SteadFast.Fields.UseSandbox.Hint"] = "Check to use sandbox (test environment).",
            ["Plugins.Shipping.SteadFast.Fields.AutoCreateShipment"] = "Auto Create Shipment",
            ["Plugins.Shipping.SteadFast.Fields.AutoCreateShipment.Hint"] = "Automatically create shipment on SteadFast when shipment is created in nopCommerce.",
            ["Plugins.Shipping.SteadFast.Fields.DefaultNote"] = "Default Note",
            ["Plugins.Shipping.SteadFast.Fields.DefaultNote.Hint"] = "Default note for shipments.",
            ["Plugins.Shipping.SteadFast.Fields.ShippingByWeightByTotalEnabled"] = "Calculate by Weight/Total",
            ["Plugins.Shipping.SteadFast.Fields.ShippingByWeightByTotalEnabled.Hint"] = "Enable shipping rate calculation by weight and order total.",
            ["Plugins.Shipping.SteadFast.Fields.LimitMethodsToCreated"] = "Limit to Configured Methods",
            ["Plugins.Shipping.SteadFast.Fields.LimitMethodsToCreated.Hint"] = "Limit shipping methods to configured ones only.",
            ["Plugins.Shipping.SteadFast.AddRecord"] = "Add record",
            ["Plugins.Shipping.SteadFast.Fields.Store"] = "Store",
            ["Plugins.Shipping.SteadFast.Fields.Store.Hint"] = "Select store or * for all stores.",
            ["Plugins.Shipping.SteadFast.Fields.Warehouse"] = "Warehouse",
            ["Plugins.Shipping.SteadFast.Fields.Warehouse.Hint"] = "Select warehouse or * for all warehouses.",
            ["Plugins.Shipping.SteadFast.Fields.Country"] = "Country",
            ["Plugins.Shipping.SteadFast.Fields.Country.Hint"] = "Select country or * for all countries.",
            ["Plugins.Shipping.SteadFast.Fields.StateProvince"] = "State/Province",
            ["Plugins.Shipping.SteadFast.Fields.StateProvince.Hint"] = "Select state/province or * for all states.",
            ["Plugins.Shipping.SteadFast.Fields.Zip"] = "Zip",
            ["Plugins.Shipping.SteadFast.Fields.Zip.Hint"] = "Enter zip code or leave empty for all.",
            ["Plugins.Shipping.SteadFast.Fields.ShippingMethod"] = "Shipping Method",
            ["Plugins.Shipping.SteadFast.Fields.ShippingMethod.Hint"] = "Select shipping method.",
            ["Plugins.Shipping.SteadFast.Fields.TransitDays"] = "Transit Days",
            ["Plugins.Shipping.SteadFast.Fields.TransitDays.Hint"] = "Number of days for delivery.",
            ["Plugins.Shipping.SteadFast.Fields.WeightFrom"] = "Weight From",
            ["Plugins.Shipping.SteadFast.Fields.WeightFrom.Hint"] = "Minimum order weight.",
            ["Plugins.Shipping.SteadFast.Fields.WeightTo"] = "Weight To",
            ["Plugins.Shipping.SteadFast.Fields.WeightTo.Hint"] = "Maximum order weight.",
            ["Plugins.Shipping.SteadFast.Fields.OrderSubtotalFrom"] = "Order Subtotal From",
            ["Plugins.Shipping.SteadFast.Fields.OrderSubtotalFrom.Hint"] = "Minimum order subtotal.",
            ["Plugins.Shipping.SteadFast.Fields.OrderSubtotalTo"] = "Order Subtotal To",
            ["Plugins.Shipping.SteadFast.Fields.OrderSubtotalTo.Hint"] = "Maximum order subtotal.",
            ["Plugins.Shipping.SteadFast.Fields.AdditionalFixedCost"] = "Additional Fixed Cost",
            ["Plugins.Shipping.SteadFast.Fields.AdditionalFixedCost.Hint"] = "Additional fixed cost for shipping.",
            ["Plugins.Shipping.SteadFast.Fields.PercentageRateOfSubtotal"] = "Percentage Rate",
            ["Plugins.Shipping.SteadFast.Fields.PercentageRateOfSubtotal.Hint"] = "Percentage rate of order subtotal.",
            ["Plugins.Shipping.SteadFast.Fields.RatePerWeightUnit"] = "Rate Per Weight Unit",
            ["Plugins.Shipping.SteadFast.Fields.RatePerWeightUnit.Hint"] = "Rate per weight unit.",
            ["Plugins.Shipping.SteadFast.Fields.LowerWeightLimit"] = "Lower Weight Limit",
            ["Plugins.Shipping.SteadFast.Fields.LowerWeightLimit.Hint"] = "Lower weight limit for rate calculation.",
            ["Plugins.Shipping.SteadFast.ManageShipments"] = "Manage Shipments",
            ["Plugins.Shipping.SteadFast.Shipment.ConsignmentId"] = "Consignment ID",
            ["Plugins.Shipping.SteadFast.Shipment.OrderId"] = "Order ID",
            ["Plugins.Shipping.SteadFast.Shipment.Status"] = "Status",
            ["Plugins.Shipping.SteadFast.Shipment.TrackingNumber"] = "Tracking Number",
            ["Plugins.Shipping.SteadFast.Shipment.CreatedOn"] = "Created On",
            ["Plugins.Shipping.SteadFast.TestConnection"] = "Test Connection",
            ["Plugins.Shipping.SteadFast.TestConnection.Success"] = "Connection successful!",
            ["Plugins.Shipping.SteadFast.TestConnection.Failed"] = "Connection failed. Please check your credentials."
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UninstallAsync()
    {
        //settings
        await _settingService.DeleteSettingAsync<SteadFastSettings>();

        //fixed rates
        var fixedRates = await (await _shippingService.GetAllShippingMethodsAsync())
            .SelectAwait(async shippingMethod => await _settingService.GetSettingAsync(
                string.Format(SteadFastDefaults.FIXED_RATE_SETTINGS_KEY, shippingMethod.Id)))
            .Where(setting => setting != null)
            .ToListAsync();
        await _settingService.DeleteSettingsAsync(fixedRates);

        //locales
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Shipping.SteadFast");

        await base.UninstallAsync();
    }

    #endregion
}
