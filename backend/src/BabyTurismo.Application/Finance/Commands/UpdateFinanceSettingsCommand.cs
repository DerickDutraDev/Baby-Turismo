using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Finance.Commands;

public sealed record UpdateFinanceSettingsCommand(decimal OwnerSalary) : IRequest<Result>;
