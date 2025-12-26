using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Shipping.SteadFast.Services;
using Nop.Services.Shipping;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Shipping.SteadFast.Components;

[ViewComponent(Name = "SteadFastShipmentDetails")]
public class SteadFastShipmentDetailsViewComponent : NopViewComponent
{
    private readonly ISteadFastShipmentRecordService _shipmentRecordService;
    private readonly IShipmentService _shipmentService;
    private readonly SteadFastSettings _steadFastSettings;

    public SteadFastShipmentDetailsViewComponent(
        ISteadFastShipmentRecordService shipmentRecordService,
        IShipmentService shipmentService,
        SteadFastSettings steadFastSettings)
    {
        _shipmentRecordService = shipmentRecordService;
        _shipmentService = shipmentService;
        _steadFastSettings = steadFastSettings;
    }

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        if (additionalData is not int shipmentId || shipmentId == 0)
            return Content("");

        var shipment = await _shipmentService.GetShipmentByIdAsync(shipmentId);
        if (shipment == null)
            return Content("");

        // Check if shipment record exists
        var shipmentRecord = await _shipmentRecordService.GetShipmentRecordByShipmentIdAsync(shipmentId);
        
        var model = new
        {
            ShipmentId = shipmentId,
            IsSent = shipmentRecord?.IsSent ?? false,
            ConsignmentId = shipmentRecord?.ConsignmentId ?? "",
            TrackingNumber = shipmentRecord?.TrackingNumber ?? ""
        };

        return View("~/Plugins/Shipping.SteadFast/Views/Components/SteadFastShipmentDetails.cshtml", model);
    }
}
