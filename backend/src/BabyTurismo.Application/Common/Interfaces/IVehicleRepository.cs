using BabyTurismo.Domain.Fleet.Vehicles;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Application.Fleet.Vehicles;
using BabyTurismo.Shared.Pagination;

namespace BabyTurismo.Application.Common.Interfaces;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<Vehicle?> GetByChassiAsync(string chassi, CancellationToken cancellationToken = default);
    Task<PagedResult<VehicleDto>> GetPaginatedVehiclesAsync(int page, int pageSize, string? searchTerm, string? status, CancellationToken cancellationToken = default);
    Task<VehicleDto?> GetVehicleByIdWithDriverAsync(Guid id, CancellationToken cancellationToken = default);
}
