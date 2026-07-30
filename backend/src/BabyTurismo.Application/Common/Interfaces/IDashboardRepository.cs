using BabyTurismo.Application.Dashboard.Queries;

namespace BabyTurismo.Application.Common.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
