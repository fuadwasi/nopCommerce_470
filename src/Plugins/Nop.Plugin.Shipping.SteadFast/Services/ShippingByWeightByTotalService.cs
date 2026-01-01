using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.Shipping.SteadFast.Domain;

namespace Nop.Plugin.Shipping.SteadFast.Services;

/// <summary>
/// Represents service for shipping by weight and by total
/// </summary>
public class ShippingByWeightByTotalService : IShippingByWeightByTotalService
{
    #region Fields

    private readonly IRepository<SteadFastShippingByWeightByTotalRecord> _shippingByWeightByTotalRepository;
    private readonly IShortTermCacheManager _shortTermCacheManager;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly SteadFastSettings _steadFastSettings;

    #endregion

    #region Ctor

    public ShippingByWeightByTotalService(
        IRepository<SteadFastShippingByWeightByTotalRecord> shippingByWeightByTotalRepository,
        IShortTermCacheManager shortTermCacheManager,
        IStaticCacheManager staticCacheManager,
        SteadFastSettings steadFastSettings)
    {
        _shippingByWeightByTotalRepository = shippingByWeightByTotalRepository;
        _shortTermCacheManager = shortTermCacheManager;
        _staticCacheManager = staticCacheManager;
        _steadFastSettings = steadFastSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get filtered shipping by weight records
    /// </summary>
    /// <param name="shippingMethodId">Shipping method identifier</param>
    /// <param name="storeId">Store identifier</param>
    /// <param name="warehouseId">Warehouse identifier</param>
    /// <param name="countryId">Country identifier</param>
    /// <param name="stateProvinceId">State identifier</param>
    /// <param name="zip">Zip postal code</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the shipping by weight record
    /// </returns>
    private async Task<IList<SteadFastShippingByWeightByTotalRecord>> GetRecordsAsync(int shippingMethodId,
        int storeId,
        int warehouseId,
        int countryId,
        int stateProvinceId,
        string zip)
    {
        var records = await _shippingByWeightByTotalRepository.GetAllAsync(query =>
        {
            //filter by shipping method
            query = query.Where(sbw => sbw.ShippingMethodId == shippingMethodId);

            //filter by store
            query = storeId == 0
                ? query
                : query.Where(r => r.StoreId == storeId || r.StoreId == 0);

            //filter by warehouse
            query = warehouseId == 0
                ? query
                : query.Where(r => r.WarehouseId == warehouseId || r.WarehouseId == 0);

            //filter by country
            query = countryId == 0
                ? query
                : query.Where(r => r.CountryId == countryId || r.CountryId == 0);

            //filter by state/province
            query = stateProvinceId == 0
                ? query
                : query.Where(r => r.StateProvinceId == stateProvinceId || r.StateProvinceId == 0);

            zip = zip?.Trim() ?? string.Empty;

            //filter by zip
            query = string.IsNullOrEmpty(zip)
                ? query
                : query.Where(r => string.IsNullOrEmpty(r.Zip) || r.Zip.Equals(zip));

            query = query.OrderBy(sbw => sbw.StoreId)
                .ThenBy(sbw => sbw.CountryId)
                .ThenBy(sbw => sbw.StateProvinceId)
                .ThenBy(sbw => sbw.Zip)
                .ThenBy(sbw => sbw.ShippingMethodId)
                .ThenBy(sbw => sbw.WeightFrom)
                .ThenBy(sbw => sbw.OrderSubtotalFrom);

            return query;
        });

        return records;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get a shipping by weight record by passed parameters
    /// </summary>
    public virtual async Task<SteadFastShippingByWeightByTotalRecord> FindRecordsAsync(int shippingMethodId, int storeId, int warehouseId,
        int countryId, int stateProvinceId, string zip, decimal weight, decimal orderSubtotal)
    {
        zip = zip?.Trim() ?? string.Empty;

        var existingRates = await GetRecordsAsync(shippingMethodId, storeId, warehouseId, countryId, stateProvinceId, zip);

        //filter by weight and order subtotal
        var matchedByWeightAndTotal = existingRates
            .Where(sbw => weight >= sbw.WeightFrom && weight <= sbw.WeightTo &&
                         orderSubtotal >= sbw.OrderSubtotalFrom && orderSubtotal <= sbw.OrderSubtotalTo)
            .ToList();

        //sort from particular to general, more particular cases will be the first
        matchedByWeightAndTotal = matchedByWeightAndTotal
            .OrderBy(r => r.StoreId == 0)
            .ThenBy(r => r.WarehouseId == 0)
            .ThenBy(r => r.CountryId == 0)
            .ThenBy(r => r.StateProvinceId == 0)
            .ThenBy(r => string.IsNullOrEmpty(r.Zip))
            .ToList();

        return matchedByWeightAndTotal.FirstOrDefault();
    }

    /// <summary>
    /// Filter Shipping Weight Records
    /// </summary>
    public virtual async Task<IPagedList<SteadFastShippingByWeightByTotalRecord>> FindRecordsAsync(int shippingMethodId, int storeId, int warehouseId,
        int countryId, int stateProvinceId, string zip, decimal? weight, decimal? orderSubtotal, int pageIndex, int pageSize)
    {
        //filter by weight
        var existingRates =
            (await GetRecordsAsync(shippingMethodId, storeId, warehouseId, countryId, stateProvinceId, zip))
            .Where(sbw => !weight.HasValue || weight >= sbw.WeightFrom && weight <= sbw.WeightTo);

        //filter by order subtotal
        existingRates = !orderSubtotal.HasValue ? existingRates :
            existingRates.Where(sbw => orderSubtotal >= sbw.OrderSubtotalFrom && orderSubtotal <= sbw.OrderSubtotalTo);

        //sort from particular to general, more particular cases will be the first
        existingRates = existingRates
            .OrderBy(r => r.StoreId == 0)
            .ThenBy(r => r.WarehouseId == 0)
            .ThenBy(r => r.CountryId == 0)
            .ThenBy(r => r.StateProvinceId == 0)
            .ThenBy(r => string.IsNullOrEmpty(r.Zip));

        var records = new PagedList<SteadFastShippingByWeightByTotalRecord>(existingRates.ToList(), pageIndex, pageSize);

        return records;
    }

    /// <summary>
    /// Get a shipping by weight record by identifier
    /// </summary>
    public virtual async Task<SteadFastShippingByWeightByTotalRecord> GetByIdAsync(int shippingByWeightRecordId)
    {
        return await _shippingByWeightByTotalRepository.GetByIdAsync(shippingByWeightRecordId, cache => default);
    }

    /// <summary>
    /// Insert the shipping by weight record
    /// </summary>
    public virtual async Task InsertShippingByWeightRecordAsync(SteadFastShippingByWeightByTotalRecord shippingByWeightRecord)
    {
        ArgumentNullException.ThrowIfNull(shippingByWeightRecord);

        await _shippingByWeightByTotalRepository.InsertAsync(shippingByWeightRecord);
    }

    /// <summary>
    /// Update the shipping by weight record
    /// </summary>
    public virtual async Task UpdateShippingByWeightRecordAsync(SteadFastShippingByWeightByTotalRecord shippingByWeightRecord)
    {
        ArgumentNullException.ThrowIfNull(shippingByWeightRecord);

        await _shippingByWeightByTotalRepository.UpdateAsync(shippingByWeightRecord);
    }

    /// <summary>
    /// Delete the shipping by weight record
    /// </summary>
    public virtual async Task DeleteShippingByWeightRecordAsync(SteadFastShippingByWeightByTotalRecord shippingByWeightRecord)
    {
        ArgumentNullException.ThrowIfNull(shippingByWeightRecord);

        await _shippingByWeightByTotalRepository.DeleteAsync(shippingByWeightRecord);
    }

    #endregion
}
