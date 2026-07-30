using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Domain.Core.Tenants;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Finance.Queries;

internal sealed class GetFinanceSettingsQueryHandler : IRequestHandler<GetFinanceSettingsQuery, Result<FinanceSettingsDto>>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantContext _tenantContext;

    public GetFinanceSettingsQueryHandler(ITenantRepository tenantRepository, ITenantContext tenantContext)
    {
        _tenantRepository = tenantRepository;
        _tenantContext = tenantContext;
    }

    public async Task<Result<FinanceSettingsDto>> Handle(GetFinanceSettingsQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(_tenantContext.TenantId, cancellationToken);
        if (tenant is null)
            return Result.Failure<FinanceSettingsDto>(Error.NotFound("Tenant.NotFound", "Tenant not found."));

        return Result.Success(new FinanceSettingsDto(tenant.OwnerSalary));
    }
}
