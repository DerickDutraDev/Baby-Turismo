using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Fleet.Fuel;
using BabyTurismo.Shared.Pagination;
using BabyTurismo.Application.Fleet.Fuel;

namespace BabyTurismo.Application.Common.Interfaces;

public interface IFuelLogRepository : IRepository<FuelLog>
{
    Task<FuelLogDto?> GetFuelLogByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<FuelLogDto>> GetPaginatedFuelLogsAsync(int page, int pageSize, Guid? vehicleId, Guid? driverId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken = default);
    Task<FuelLog?> GetLastFuelLogForVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default);
}
