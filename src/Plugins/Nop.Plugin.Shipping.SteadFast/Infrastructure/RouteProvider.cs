using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Shipping.SteadFast.Infrastructure;

/// <summary>
/// Represents plugin route provider
/// </summary>
public class RouteProvider : IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(
            name: SteadFastDefaults.ConfigurationRouteName,
            pattern: "Admin/SteadFast/Configure",
            defaults: new { controller = "SteadFast", action = "Configure" });

        endpointRouteBuilder.MapControllerRoute(
            name: SteadFastDefaults.ShipmentListRouteName,
            pattern: "Admin/SteadFast/ShipmentList",
            defaults: new { controller = "SteadFast", action = "ShipmentList" });

        endpointRouteBuilder.MapControllerRoute(
            name: SteadFastDefaults.WebhookRouteName,
            pattern: SteadFastDefaults.WEBHOOK_PATH,
            defaults: new { controller = "SteadFastWebhook", action = "WebhookHandler" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}
