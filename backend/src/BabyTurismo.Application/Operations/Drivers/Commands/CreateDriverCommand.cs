using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Drivers.Commands;

public sealed record CreateDriverCommand(
    string Name,
    string Email,
    string Password,
    string Cpf,
    string CnhNumber,
    string CnhCategory,
    DateTime CnhExpirationDate) : IRequest<Result<Guid>>;
