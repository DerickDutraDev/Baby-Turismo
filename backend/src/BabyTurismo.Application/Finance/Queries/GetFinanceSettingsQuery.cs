using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Finance.Queries;

public sealed record GetFinanceSettingsQuery : IRequest<Result<FinanceSettingsDto>>;
