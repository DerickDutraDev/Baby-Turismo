using BabyTurismo.Shared.Pagination;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Trips.Queries;

public sealed record GetTripsQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    string? Status,
    Guid? DriverId,
    Guid? VehicleId) : IRequest<Result<PagedResult<TripDto>>>;
