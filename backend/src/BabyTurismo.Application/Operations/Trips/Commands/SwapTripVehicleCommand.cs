using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Trips.Commands;

public sealed record SwapTripVehicleCommand(Guid TripId, Guid NewVehicleId) : IRequest<Result>;
