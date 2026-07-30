using BabyTurismo.Domain.Operations.Drivers;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Shared.Pagination;
using BabyTurismo.Application.Operations.Drivers;

namespace BabyTurismo.Application.Common.Interfaces;

public interface IDriverRepository : IRepository<Driver>
{
    Task<Driver?> GetByCnhAsync(string cnhNumber, CancellationToken cancellationToken = default);
    Task<Driver?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<DriverDto?> GetDriverByIdWithUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<DriverDto>> GetPaginatedDriversAsync(int page, int pageSize, string? searchTerm, string? status, CancellationToken cancellationToken = default);
}
