using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Dashboard.Queries;

public sealed record GetDashboardSummaryQuery() : IRequest<Result<DashboardSummaryDto>>;
