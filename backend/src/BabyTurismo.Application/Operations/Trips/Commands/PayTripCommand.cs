using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Trips.Commands;

public sealed record PayTripCommand(Guid Id) : IRequest<Result>;
