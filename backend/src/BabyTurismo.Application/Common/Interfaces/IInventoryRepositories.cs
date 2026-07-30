using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Inventory;
using BabyTurismo.Shared.Pagination;
using BabyTurismo.Application.Inventory;

namespace BabyTurismo.Application.Common.Interfaces;

public interface IProductCategoryRepository : IRepository<ProductCategory>
{
    Task<IReadOnlyList<ProductCategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
}

public interface IProductRepository : IRepository<Product>
{
    Task<ProductDto?> GetProductByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<ProductDto>> GetPaginatedProductsAsync(int page, int pageSize, string? searchTerm, Guid? categoryId, CancellationToken cancellationToken = default);
}

public interface IStockBalanceRepository : IRepository<StockBalance>
{
    Task<StockBalance?> GetStockBalanceAsync(Guid productId, LocationType locationType, Guid? vehicleId, CancellationToken cancellationToken = default);
    Task<PagedResult<StockBalanceDto>> GetMainStockAsync(int page, int pageSize, string? searchTerm, CancellationToken cancellationToken = default);
    Task<PagedResult<StockBalanceDto>> GetVehicleStockAsync(Guid vehicleId, int page, int pageSize, string? searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockBalanceDto>> GetStockAlertsAsync(CancellationToken cancellationToken = default);
}

public interface IInventoryMovementRepository : IRepository<InventoryMovement>
{
    Task<PagedResult<InventoryMovementDto>> GetMovementsByProductAsync(Guid productId, int page, int pageSize, CancellationToken cancellationToken = default);
}
