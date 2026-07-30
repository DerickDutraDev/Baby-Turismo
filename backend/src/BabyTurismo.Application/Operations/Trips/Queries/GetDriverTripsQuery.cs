using BabyTurismo.Shared.Pagination;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Trips.Queries;

public sealed record GetDriverTripsQuery() : IRequest<Result<PagedResult<TripDto>>>;
