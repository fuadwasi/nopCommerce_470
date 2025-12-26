using Nop.Plugin.Shipping.SteadFast.Models.Api;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents SteadFast API client interface
/// </summary>
public interface ISteadFastApiClient
{
    /// <summary>
    /// Create order on SteadFast
    /// </summary>
    /// <param name="request">Create order request</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the create order response
    /// </returns>
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);

    /// <summary>
    /// Get balance from SteadFast
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the balance response
    /// </returns>
    Task<GetBalanceResponse> GetBalanceAsync();

    /// <summary>
    /// Get status by consignment ID
    /// </summary>
    /// <param name="consignmentId">Consignment ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the status response
    /// </returns>
    Task<StatusByConsignmentIdResponse> GetStatusByConsignmentIdAsync(string consignmentId);
}
