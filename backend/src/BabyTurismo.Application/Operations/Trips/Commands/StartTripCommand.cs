using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Trips.Commands;

public sealed record StartTripCommand(Guid Id) : IRequest<Result>;
