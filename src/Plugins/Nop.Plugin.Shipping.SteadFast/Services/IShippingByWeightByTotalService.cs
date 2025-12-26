using Nop.Core;
using Nop.Plugin.Shipping.SteadFast.Domain;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents service for shipping by weight and by total
/// </summary>
public interface IShippingByWeightByTotalService
{
    /// <summary>
    /// Insert shipping by weight/total record
    /// </summary>
    /// <param name="shippingByWeightByTotalRecord">Shipping by weight/total record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertSteadFastShippingByWeightByTotalRecordAsync(SteadFastShippingByWeightByTotalRecord shippingByWeightByTotalRecord);

    /// <summary>
    /// Update shipping by weight/total record
    /// </summary>
    /// <param name="shippingByWeightByTotalRecord">Shipping by weight/total record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateSteadFastShippingByWeightByTotalRecordAsync(SteadFastShippingByWeightByTotalRecord shippingByWeightByTotalRecord);

    /// <summary>
    /// Delete shipping by weight/total record
    /// </summary>
    /// <param name="shippingByWeightByTotalRecord">Shipping by weight/total record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task DeleteSteadFastShippingByWeightByTotalRecordAsync(SteadFastShippingByWeightByTotalRecord shippingByWeightByTotalRecord);

    /// <summary>
    /// Get shipping by weight/total records
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping by weight/total records
    /// </returns>
    Task<IPagedList<SteadFastShippingByWeightByTotalRecord>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue);

    /// <summary>
    /// Find shipping by weight/total record
    /// </summary>
    /// <param name="shippingMethodId">Shipping method ID</param>
    /// <param name="storeId">Store ID</param>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="countryId">Country ID</param>
    /// <param name="stateProvinceId">State/province ID</param>
    /// <param name="zip">Zip code</param>
    /// <param name="weight">Weight</param>
    /// <param name="orderSubtotal">Order subtotal</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping by weight/total record
    /// </returns>
    Task<SteadFastShippingByWeightByTotalRecord> FindRecordsAsync(
        int shippingMethodId,
        int storeId,
        int warehouseId,
        int countryId,
        int stateProvinceId,
        string zip,
        decimal weight,
        decimal orderSubtotal);

    /// <summary>
    /// Get shipping by weight/total record by ID
    /// </summary>
    /// <param name="id">Record ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping by weight/total record
    /// </returns>
    Task<SteadFastShippingByWeightByTotalRecord> GetByIdAsync(int id);
}
