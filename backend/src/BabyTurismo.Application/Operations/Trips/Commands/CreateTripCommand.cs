using BabyTurismo.Domain.Operations.Trips;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Trips.Commands;

public sealed record CreateTripCommand(
    Guid DriverId,
    Guid VehicleId,
    string Origin,
    string Destination,
    DateTime ScheduledStartDate,
    DateTime ScheduledEndDate,
    decimal TripValue,
    PaymentStatus PaymentStatus,
    string? Notes) : IRequest<Result<Guid>>;
