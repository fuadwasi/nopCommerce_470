using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Shipping.SteadFast.Models.Admin;

/// <summary>
/// Represents a shipment list model
/// </summary>
public record ShipmentListModel : BaseNopModel
{
    #region Properties

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Shipment.OrderId")]
    public int? OrderId { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Shipment.ConsignmentId")]
    public string ConsignmentId { get; set; }

    #endregion
}

/// <summary>
/// Represents a shipment model
/// </summary>
public record ShipmentModel : BaseNopModel
{
    #region Properties

    public int Id { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Shipment.OrderId")]
    public int OrderId { get; set; }

    public int ShipmentId { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Shipment.ConsignmentId")]
    public string ConsignmentId { get; set; }

    public string InvoiceNumber { get; set; }

    public string RecipientName { get; set; }

    public string RecipientPhone { get; set; }

    public string RecipientAddress { get; set; }

    public decimal CodAmount { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Shipment.Status")]
    public string DeliveryStatus { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Shipment.TrackingNumber")]
    public string TrackingNumber { get; set; }

    public bool IsSent { get; set; }

    [NopResourceDisplayName("Plugins.Shipping.SteadFast.Shipment.CreatedOn")]
    public DateTime CreatedOnUtc { get; set; }

    #endregion
}
