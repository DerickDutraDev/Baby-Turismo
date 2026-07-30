using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Fleet.Maintenance;
using BabyTurismo.Shared.Pagination;
using BabyTurismo.Application.Fleet.Maintenance;

namespace BabyTurismo.Application.Common.Interfaces;

public interface IMaintenanceRepository : IRepository<MaintenanceRecord>
{
    Task<MaintenanceDto?> GetMaintenanceByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<MaintenanceDto>> GetPaginatedMaintenancesAsync(int page, int pageSize, Guid? vehicleId, MaintenanceType? type, MaintenanceStatus? status, CancellationToken cancellationToken = default);
}
