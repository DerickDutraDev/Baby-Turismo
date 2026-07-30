using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Trips.Queries;

public sealed record GetTripByIdQuery(Guid Id) : IRequest<Result<TripDto>>;
