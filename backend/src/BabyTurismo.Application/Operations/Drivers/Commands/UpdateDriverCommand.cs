using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Drivers.Commands;

public sealed record UpdateDriverCommand(
    Guid Id,
    string CnhNumber,
    string CnhCategory,
    DateTime CnhExpirationDate) : IRequest<Result>;
