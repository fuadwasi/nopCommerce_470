using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Shipping.SteadFast.Models.Admin;
using Nop.Plugin.Shipping.SteadFast.Models.Api;
using Nop.Plugin.Shipping.SteadFast.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Shipping.SteadFast.Controllers;

[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class SteadFastController : BasePluginController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly ISteadFastService _steadFastService;
    private readonly IOrderService _orderService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly IAddressService _addressService;

    #endregion

    #region Ctor

    public SteadFastController(
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        ISteadFastService steadFastService,
        IOrderService orderService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IAddressService addressService)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _steadFastService = steadFastService;
        _orderService = orderService;
        _storeContext = storeContext;
        _workContext = workContext;
        _addressService = addressService;
    }

    #endregion

    #region Methods

    public async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
            return AccessDeniedView();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<SteadFastSettings>(storeScope);

        var model = new ConfigurationModel
        {
            ApiKey = settings.ApiKey,
            ApiSecretKey = settings.ApiSecretKey,
            UseSandbox = settings.UseSandbox,
            AutoCreateShipment = settings.AutoCreateShipment,
            DefaultNote = settings.DefaultNote,
            ShippingByWeightByTotalEnabled = settings.ShippingByWeightByTotalEnabled,
            LimitMethodsToCreated = settings.LimitMethodsToCreated
        };

        //try to get current balance
        try
        {
            if (!string.IsNullOrEmpty(settings.ApiKey) && !string.IsNullOrEmpty(settings.ApiSecretKey))
            {
                var balanceResponse = await _steadFastService.GetBalanceAsync();
                if (balanceResponse?.Status == 200)
                {
                    model.CurrentBalance = balanceResponse.CurrentBalance;
                }
            }
        }
        catch
        {
            //ignore
        }

        return View("~/Plugins/Shipping.SteadFast/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
            return AccessDeniedView();

        if (!ModelState.IsValid)
            return await Configure();

        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var settings = await _settingService.LoadSettingAsync<SteadFastSettings>(storeScope);

        settings.ApiKey = model.ApiKey;
        settings.ApiSecretKey = model.ApiSecretKey;
        settings.UseSandbox = model.UseSandbox;
        settings.AutoCreateShipment = model.AutoCreateShipment;
        settings.DefaultNote = model.DefaultNote;
        settings.ShippingByWeightByTotalEnabled = model.ShippingByWeightByTotalEnabled;
        settings.LimitMethodsToCreated = model.LimitMethodsToCreated;

        await _settingService.SaveSettingAsync(settings, storeScope);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(
            await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    public async Task<IActionResult> TestConnection()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
            return AccessDeniedView();

        try
        {
            var balanceResponse = await _steadFastService.GetBalanceAsync();
            if (balanceResponse?.Status == 200)
            {
                return Json(new
                {
                    success = true,
                    message = await _localizationService.GetResourceAsync("Plugins.Shipping.SteadFast.TestConnection.Success"),
                    balance = balanceResponse.CurrentBalance
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = await _localizationService.GetResourceAsync("Plugins.Shipping.SteadFast.TestConnection.Failed")
                });
            }
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    public async Task<IActionResult> ShipmentList()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return AccessDeniedView();

        var model = new ShipmentListModel();

        return View("~/Plugins/Shipping.SteadFast/Views/ShipmentList.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> ShipmentList(ShipmentListModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return await AccessDeniedDataTablesJson();

        var shipments = await _steadFastService.GetAllShipmentRecordsAsync(
            orderId: searchModel.OrderId,
            consignmentId: searchModel.ConsignmentId);

        var model = new
        {
            Data = shipments.Select(s => new ShipmentModel
            {
                Id = s.Id,
                OrderId = s.OrderId,
                ShipmentId = s.ShipmentId,
                ConsignmentId = s.ConsignmentId,
                InvoiceNumber = s.InvoiceNumber,
                RecipientName = s.RecipientName,
                RecipientPhone = s.RecipientPhone,
                RecipientAddress = s.RecipientAddress,
                CodAmount = s.CodAmount,
                DeliveryStatus = s.DeliveryStatus,
                TrackingNumber = s.TrackingNumber,
                IsSent = s.IsSent,
                CreatedOnUtc = s.CreatedOnUtc
            }),
            Total = shipments.TotalCount
        };

        return Json(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateShipment(int orderId)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return AccessDeniedView();

        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            //get shipping address
            var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);
            if (shippingAddress == null)
                return Json(new { success = false, message = "Shipping address not found" });

            //check if shipment already exists
            var existingShipment = await _steadFastService.GetShipmentRecordByOrderIdAsync(orderId);
            if (existingShipment != null)
                return Json(new { success = false, message = "Shipment already exists for this order" });

            //prepare request
            var request = new CreateOrderRequest
            {
                Invoice = $"{DateTime.UtcNow:yyMMdd}-{order.Id}",
                RecipientName = $"{shippingAddress.FirstName} {shippingAddress.LastName}",
                RecipientPhone = shippingAddress.PhoneNumber ?? "",
                RecipientAddress = $"{shippingAddress.Address1}, {shippingAddress.City}-{shippingAddress.ZipPostalCode}",
                CodAmount = order.OrderTotal,
                Note = ""
            };

            //create order on SteadFast
            var response = await _steadFastService.CreateOrderAsync(request);

            if (response.Status != 200)
            {
                return Json(new { success = false, message = response.Message ?? "Failed to create shipment" });
            }

            //save shipment record
            var shipmentRecord = new Domain.SteadFastShipmentRecord
            {
                ShipmentId = 0,
                OrderId = order.Id,
                ConsignmentId = response.Consignment?.ConsignmentId ?? "",
                InvoiceNumber = request.Invoice,
                RecipientName = request.RecipientName,
                RecipientPhone = request.RecipientPhone,
                RecipientAddress = request.RecipientAddress,
                CodAmount = request.CodAmount,
                Note = request.Note,
                DeliveryStatus = response.Consignment?.Status ?? "",
                TrackingNumber = response.Consignment?.TrackingCode ?? "",
                IsSent = true,
                ApiResponse = System.Text.Json.JsonSerializer.Serialize(response),
                CreatedOnUtc = DateTime.UtcNow
            };

            await _steadFastService.InsertShipmentRecordAsync(shipmentRecord);

            return Json(new
            {
                success = true,
                message = "Shipment created successfully",
                consignmentId = response.Consignment?.ConsignmentId
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateShipmentStatus(string consignmentId)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return AccessDeniedView();

        try
        {
            var response = await _steadFastService.GetStatusByConsignmentIdAsync(consignmentId);

            if (response.Status != 200)
            {
                return Json(new { success = false, message = response.Message ?? "Failed to get status" });
            }

            //update shipment record
            var shipments = await _steadFastService.GetAllShipmentRecordsAsync(consignmentId: consignmentId);
            var shipment = shipments.FirstOrDefault();

            if (shipment != null)
            {
                shipment.DeliveryStatus = response.DeliveryStatus;
                shipment.UpdatedOnUtc = DateTime.UtcNow;
                await _steadFastService.UpdateShipmentRecordAsync(shipment);
            }

            return Json(new
            {
                success = true,
                message = "Status updated successfully",
                status = response.DeliveryStatus
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion
}
