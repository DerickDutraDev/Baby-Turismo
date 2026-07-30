using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Operations.Trips;
using BabyTurismo.Shared.Pagination;
using BabyTurismo.Application.Operations.Trips;

namespace BabyTurismo.Application.Common.Interfaces;

public interface ITripRepository : IRepository<Trip>
{
    Task<TripDto?> GetTripByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<TripDto>> GetPaginatedTripsAsync(int page, int pageSize, string? searchTerm, string? status, Guid? driverId, Guid? vehicleId, CancellationToken cancellationToken = default);
    Task<TripDto?> GetActiveTripByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TripDto>> GetTripsByDriverIdAsync(Guid driverId, int take = 50, CancellationToken cancellationToken = default);
}
