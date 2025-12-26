using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Plugin.Shipping.SteadFast.Domain;
using Nop.Plugin.Shipping.SteadFast.Models.Api;
using Nop.Plugin.Shipping.SteadFast.Services;
using Nop.Services.Events;
using Nop.Services.Orders;
using Nop.Services.Logging;

namespace Nop.Plugin.Shipping.SteadFast.Infrastructure.Cache;

/// <summary>
/// Represents shipment event consumer
/// </summary>
public class ShipmentEventConsumer : IConsumer<EntityInsertedEvent<Shipment>>
{
    #region Fields

    private readonly ISteadFastService _steadFastService;
    private readonly IOrderService _orderService;
    private readonly ILogger _logger;
    private readonly SteadFastSettings _steadFastSettings;

    #endregion

    #region Ctor

    public ShipmentEventConsumer(
        ISteadFastService steadFastService,
        IOrderService orderService,
        ILogger logger,
        SteadFastSettings steadFastSettings)
    {
        _steadFastService = steadFastService;
        _orderService = orderService;
        _logger = logger;
        _steadFastSettings = steadFastSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle the shipment inserted event
    /// </summary>
    /// <param name="eventMessage">Event message</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(EntityInsertedEvent<Shipment> eventMessage)
    {
        if (eventMessage?.Entity == null)
            return;

        //check if auto create shipment is enabled
        if (!_steadFastSettings.AutoCreateShipment)
            return;

        var shipment = eventMessage.Entity;
        var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

        if (order == null)
            return;

        //check if order is not pickup in store
        if (order.PickupInStore)
            return;

        //check if order's shipping rate computation method is SteadFast
        if (order.ShippingRateComputationMethodSystemName != SteadFastDefaults.SystemName)
            return;

        try
        {
            //prepare request
            var request = new CreateOrderRequest
            {
                Invoice = $"{DateTime.UtcNow:yyMMdd}-{order.Id}",
                RecipientName = $"{order.ShippingAddress?.FirstName} {order.ShippingAddress?.LastName}",
                RecipientPhone = order.ShippingAddress?.PhoneNumber ?? "",
                RecipientAddress = $"{order.ShippingAddress?.Address1}, {order.ShippingAddress?.City}-{order.ShippingAddress?.ZipPostalCode}",
                CodAmount = order.OrderTotal,
                Note = _steadFastSettings.DefaultNote ?? ""
            };

            //create order on SteadFast
            var response = await _steadFastService.CreateOrderAsync(request);

            //save shipment record
            var shipmentRecord = new SteadFastShipmentRecord
            {
                ShipmentId = shipment.Id,
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
                IsSent = response.Status == 200,
                ApiResponse = System.Text.Json.JsonSerializer.Serialize(response),
                CreatedOnUtc = DateTime.UtcNow
            };

            await _steadFastService.InsertShipmentRecordAsync(shipmentRecord);

            //update shipment tracking number
            if (response.Status == 200 && !string.IsNullOrEmpty(response.Consignment?.TrackingCode))
            {
                shipment.TrackingNumber = response.Consignment.TrackingCode;
                await _orderService.UpdateShipmentAsync(shipment);
            }
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"SteadFast: Error creating shipment for order {order.Id}", ex);
        }
    }

    #endregion
}
