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
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly SteadFastSettings _steadFastSettings;

    #endregion

    #region Ctor

    public ShippingByWeightByTotalService(
        IRepository<SteadFastShippingByWeightByTotalRecord> shippingByWeightByTotalRepository,
        IStaticCacheManager staticCacheManager,
        SteadFastSettings steadFastSettings)
    {
        _shippingByWeightByTotalRepository = shippingByWeightByTotalRepository;
        _staticCacheManager = staticCacheManager;
        _steadFastSettings = steadFastSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Insert shipping by weight/total record
    /// </summary>
    /// <param name="shippingByWeightByTotalRecord">Shipping by weight/total record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task InsertSteadFastShippingByWeightByTotalRecordAsync(SteadFastShippingByWeightByTotalRecord shippingByWeightByTotalRecord)
    {
        ArgumentNullException.ThrowIfNull(shippingByWeightByTotalRecord);

        await _shippingByWeightByTotalRepository.InsertAsync(shippingByWeightByTotalRecord);
    }

    /// <summary>
    /// Update shipping by weight/total record
    /// </summary>
    /// <param name="shippingByWeightByTotalRecord">Shipping by weight/total record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task UpdateSteadFastShippingByWeightByTotalRecordAsync(SteadFastShippingByWeightByTotalRecord shippingByWeightByTotalRecord)
    {
        ArgumentNullException.ThrowIfNull(shippingByWeightByTotalRecord);

        await _shippingByWeightByTotalRepository.UpdateAsync(shippingByWeightByTotalRecord);
    }

    /// <summary>
    /// Delete shipping by weight/total record
    /// </summary>
    /// <param name="shippingByWeightByTotalRecord">Shipping by weight/total record</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public virtual async Task DeleteSteadFastShippingByWeightByTotalRecordAsync(SteadFastShippingByWeightByTotalRecord shippingByWeightByTotalRecord)
    {
        ArgumentNullException.ThrowIfNull(shippingByWeightByTotalRecord);

        await _shippingByWeightByTotalRepository.DeleteAsync(shippingByWeightByTotalRecord);
    }

    /// <summary>
    /// Get shipping by weight/total records
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping by weight/total records
    /// </returns>
    public virtual async Task<IPagedList<SteadFastShippingByWeightByTotalRecord>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var records = await _shippingByWeightByTotalRepository.GetAllAsync(query =>
        {
            return from record in query
                   orderby record.StoreId, record.CountryId, record.StateProvinceId, record.Zip,
                       record.ShippingMethodId, record.WeightFrom, record.OrderSubtotalFrom
                   select record;
        });

        return new PagedList<SteadFastShippingByWeightByTotalRecord>(records, pageIndex, pageSize);
    }

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
    public virtual async Task<SteadFastShippingByWeightByTotalRecord> FindRecordsAsync(
        int shippingMethodId,
        int storeId,
        int warehouseId,
        int countryId,
        int stateProvinceId,
        string zip,
        decimal weight,
        decimal orderSubtotal)
    {
        if (!_steadFastSettings.LoadAllRecord)
        {
            zip ??= string.Empty;
            zip = zip.Trim();

            //filter by weight and shipping method
            var existingRates = await _shippingByWeightByTotalRepository.Table
                .Where(record => record.ShippingMethodId == shippingMethodId && weight >= record.WeightFrom && weight <= record.WeightTo)
                .ToListAsync();

            //filter by order subtotal
            existingRates = existingRates.Where(record => orderSubtotal >= record.OrderSubtotalFrom && orderSubtotal <= record.OrderSubtotalTo).ToList();

            //filter by store
            existingRates = existingRates.Where(record => record.StoreId == storeId || record.StoreId == 0).ToList();

            //filter by warehouse
            existingRates = existingRates.Where(record => record.WarehouseId == warehouseId || record.WarehouseId == 0).ToList();

            //filter by country
            existingRates = existingRates.Where(record => record.CountryId == countryId || record.CountryId == 0).ToList();

            //filter by state/province
            existingRates = existingRates.Where(record => record.StateProvinceId == stateProvinceId || record.StateProvinceId == 0).ToList();

            //filter by zip
            existingRates = existingRates.Where(record => string.IsNullOrEmpty(record.Zip) || record.Zip.Equals(zip, StringComparison.InvariantCultureIgnoreCase)).ToList();

            //sort
            existingRates = existingRates.OrderBy(record => record.StoreId)
                .ThenBy(record => record.WarehouseId)
                .ThenBy(record => record.CountryId)
                .ThenBy(record => record.StateProvinceId)
                .ThenBy(record => record.Zip)
                .ToList();

            return existingRates.FirstOrDefault();
        }

        //load all records to cache
        var key = new Nop.Core.Caching.CacheKey("Nop.Plugin.Shipping.SteadFast.ShippingByWeightByTotal.All");
        var allRecords = await _staticCacheManager.GetAsync(key, async () => await _shippingByWeightByTotalRepository.GetAllAsync(query => query));

        zip ??= string.Empty;
        zip = zip.Trim();

        var foundRecords = allRecords
            .Where(record =>
                record.ShippingMethodId == shippingMethodId &&
                weight >= record.WeightFrom && weight <= record.WeightTo &&
                orderSubtotal >= record.OrderSubtotalFrom && orderSubtotal <= record.OrderSubtotalTo &&
                (record.StoreId == storeId || record.StoreId == 0) &&
                (record.WarehouseId == warehouseId || record.WarehouseId == 0) &&
                (record.CountryId == countryId || record.CountryId == 0) &&
                (record.StateProvinceId == stateProvinceId || record.StateProvinceId == 0) &&
                (string.IsNullOrEmpty(record.Zip) || record.Zip.Equals(zip, StringComparison.InvariantCultureIgnoreCase)))
            .OrderBy(record => record.StoreId)
            .ThenBy(record => record.WarehouseId)
            .ThenBy(record => record.CountryId)
            .ThenBy(record => record.StateProvinceId)
            .ThenBy(record => record.Zip)
            .ToList();

        return foundRecords.FirstOrDefault();
    }

    /// <summary>
    /// Get shipping by weight/total record by ID
    /// </summary>
    /// <param name="id">Record ID</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the shipping by weight/total record
    /// </returns>
    public virtual async Task<SteadFastShippingByWeightByTotalRecord> GetByIdAsync(int id)
    {
        return await _shippingByWeightByTotalRepository.GetByIdAsync(id);
    }

    #endregion
}
