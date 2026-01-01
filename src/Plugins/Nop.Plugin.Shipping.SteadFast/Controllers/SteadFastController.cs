using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Shipping.SteadFast.Models.Admin;
using Nop.Plugin.Shipping.SteadFast.Models.Api;
using Nop.Plugin.Shipping.SteadFast.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
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
    private readonly ICustomerService _customerService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;
    private readonly ISteadFastService _steadFastService;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IStoreContext _storeContext;
    private readonly IWorkContext _workContext;
    private readonly IAddressService _addressService;
    private readonly ISteadFastShipmentEventLogService _eventLogService;
    private readonly IShipmentService _shipmentService;

    #endregion

    #region Ctor

    public SteadFastController(
        ILocalizationService localizationService,
        ICustomerService customerService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        ISteadFastService steadFastService,
        IOrderService orderService,
        IProductService productService,
        IStoreContext storeContext,
        IWorkContext workContext,
        IAddressService addressService,
        ISteadFastShipmentEventLogService eventLogService,
        IShipmentService shipmentService)
    {
        _localizationService = localizationService;
        _customerService = customerService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
        _steadFastService = steadFastService;
        _orderService = orderService;
        _productService = productService;
        _storeContext = storeContext;
        _workContext = workContext;
        _addressService = addressService;
        _eventLogService = eventLogService;
        _shipmentService = shipmentService;
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
            LimitMethodsToCreated = settings.LimitMethodsToCreated,
            WebhookEnabled = settings.WebhookEnabled,
            WebhookSecret = settings.WebhookSecret,
            WebhookUrl = $"{Request.Scheme}://{Request.Host}/{SteadFastDefaults.WEBHOOK_PATH}"
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
        settings.WebhookEnabled = model.WebhookEnabled;
        settings.WebhookSecret = model.WebhookSecret;

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
                Invoice = $"{order.CustomOrderNumber}",
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

    [HttpGet]
    public async Task<IActionResult> GetShipmentInfo(int shipmentId)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return AccessDeniedView();

        try
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
                return Json(new { success = false, message = "Shipment not found" });

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            var shippingAddress = await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? 0);

            // Calculate COD amount from shipment items
            decimal codAmount = 0;
            var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipmentId);
            foreach (var item in shipmentItems)
            {
                var orderItem = await _orderService.GetOrderItemByIdAsync(item.OrderItemId);
                if (orderItem != null)
                {
                    codAmount += orderItem.UnitPriceInclTax * item.Quantity;
                }
            }

            return Json(new
            {
                success = true,
                recipientName = shippingAddress != null ? $"{shippingAddress.FirstName} {shippingAddress.LastName}" : "",
                recipientPhone = shippingAddress?.PhoneNumber ?? "",
                recipientAddress = shippingAddress != null ? $"{shippingAddress.Address1}, {shippingAddress.City}-{shippingAddress.ZipPostalCode}" : "",
                codAmount = codAmount,
                orderId = order.Id
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateShipmentManual(int shipmentId, string recipientName, string recipientPhone, string recipientAddress, decimal codAmount)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return AccessDeniedView();

        try
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
            if (shipment == null)
                return Json(new { success = false, message = "Shipment not found" });

            // Check if already created
            var existingRecord = await _steadFastService.GetShipmentRecordByShipmentIdAsync(shipmentId);
            if (existingRecord != null && existingRecord.IsSent)
                return Json(new { success = false, message = "Shipment already created on SteadFast" });

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipmentId);
            if(shipmentItems == null)
                return Json(new { success = false, message = "Shipment items not found" });

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
            if (orderItems == null)
                return Json(new { success = false, message = "Order items not found" });

            var shippedOrderItems = orderItems.Where(oi => shipmentItems.Any(si => si.OrderItemId == oi.Id)).ToList();

            var products = await _productService.GetProductsByIdsAsync(shippedOrderItems.Select(i => i.ProductId).ToArray());


            // Prepare request
            var request = new CreateOrderRequest
            {
                Invoice = $"{order.CustomOrderNumber}-{shipment.Id}",
                RecipientName = recipientName,
                RecipientPhone = recipientPhone,
                RecipientAddress = recipientAddress,
                RecipientEmail = customer?.Email ?? "",
                AlternativePhone = customer?.Phone,
                CodAmount = codAmount,
                Note = "",
                ItemDescription = products != null && products.Count > 0
                    ? string.Join(", ", products.Select(p => p.Name).Take(5)) +
                      (products.Count > 5 ? ", etc." : "")
                    : ""
            };

            // Create order on SteadFast
            var response = await _steadFastService.CreateOrderAsync(request);

            if (response.Status != 200)
            {
                return Json(new { success = false, message = response.Message ?? "Failed to create shipment" });
            }

            // Save or update shipment record
            if (existingRecord != null)
            {
                existingRecord.ConsignmentId = response.Consignment?.ConsignmentId ?? "";
                existingRecord.InvoiceNumber = request.Invoice;
                existingRecord.RecipientName = request.RecipientName;
                existingRecord.RecipientPhone = request.RecipientPhone;
                existingRecord.RecipientAddress = request.RecipientAddress;
                existingRecord.CodAmount = request.CodAmount;
                existingRecord.DeliveryStatus = response.Consignment?.Status ?? "";
                existingRecord.TrackingNumber = response.Consignment?.TrackingCode ?? "";
                existingRecord.IsSent = true;
                existingRecord.ApiResponse = System.Text.Json.JsonSerializer.Serialize(response);
                existingRecord.UpdatedOnUtc = DateTime.UtcNow;
                await _steadFastService.UpdateShipmentRecordAsync(existingRecord);
            }
            else
            {
                var shipmentRecord = new Domain.SteadFastShipmentRecord
                {
                    ShipmentId = shipmentId,
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
            }

            return Json(new
            {
                success = true,
                message = "Shipment created successfully",
                consignmentId = response.Consignment?.ConsignmentId,
                trackingNumber = response.Consignment?.TrackingCode
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetShipmentEventLogs(int shipmentId)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
            return AccessDeniedView();

        try
        {
            var logs = await _eventLogService.GetEventLogsByShipmentIdAsync(shipmentId);
            
            // Get shipment record for tracking URL
            var shipmentRecord = await _steadFastService.GetShipmentRecordByShipmentIdAsync(shipmentId);
            string trackingUrl = "";
            if (shipmentRecord != null && !string.IsNullOrEmpty(shipmentRecord.TrackingNumber))
            {
                trackingUrl = $"https://steadfast.com.bd/t/{shipmentRecord.TrackingNumber}";
            }

            var logData = logs.Select(log => new
            {
                id = log.Id,
                eventType = log.EventType,
                oldStatus = log.OldStatus,
                newStatus = log.NewStatus,
                statusMessage = log.StatusMessage,
                createdOn = log.CreatedOnUtc.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return Json(new
            {
                success = true,
                logs = logData,
                trackingUrl = trackingUrl,
                consignmentId = shipmentRecord?.ConsignmentId ?? "",
                isSent = shipmentRecord?.IsSent ?? false
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion
}
