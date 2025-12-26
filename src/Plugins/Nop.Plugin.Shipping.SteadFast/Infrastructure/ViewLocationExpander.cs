using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.Shipping.SteadFast.Infrastructure;

/// <summary>
/// Represents view location expander
/// </summary>
public class ViewLocationExpander : IViewLocationExpander
{
    /// <summary>
    /// Populate values
    /// </summary>
    /// <param name="context">View location expander context</param>
    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }

    /// <summary>
    /// Expand view locations
    /// </summary>
    /// <param name="context">View location expander context</param>
    /// <param name="viewLocations">View locations</param>
    /// <returns>Expanded view locations</returns>
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName == "Admin")
        {
            viewLocations = new[] { $"/Plugins/Shipping.SteadFast/Views/{{0}}.cshtml" }.Concat(viewLocations);
        }
        return viewLocations;
    }
}
