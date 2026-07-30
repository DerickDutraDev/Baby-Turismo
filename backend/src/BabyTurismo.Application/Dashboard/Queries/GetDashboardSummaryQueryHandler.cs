using BabyTurismo.Application.Common.Interfaces;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Dashboard.Queries;

internal sealed class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, Result<DashboardSummaryDto>>
{
    private readonly IDashboardRepository _repository;
    private readonly ITenantContext _tenantContext;

    public GetDashboardSummaryQueryHandler(IDashboardRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<Result<DashboardSummaryDto>> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await _repository.GetSummaryAsync(cancellationToken);
        return Result.Success(summary);
    }
}
