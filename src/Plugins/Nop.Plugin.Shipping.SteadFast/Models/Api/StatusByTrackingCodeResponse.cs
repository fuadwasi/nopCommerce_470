using Newtonsoft.Json;

namespace Nop.Plugin.Shipping.SteadFast.Models.Api;

public class StatusByTrackingCodeResponse
{
    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("delivery_status")]
    public string DeliveryStatus { get; set; }
}
