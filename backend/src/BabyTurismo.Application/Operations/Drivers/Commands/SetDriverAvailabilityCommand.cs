using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Drivers.Commands;

public sealed record SetDriverAvailabilityCommand(Guid Id, bool IsAvailable) : IRequest<Result>;
