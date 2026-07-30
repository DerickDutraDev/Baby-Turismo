using BabyTurismo.Domain.Common.Interfaces;

namespace BabyTurismo.Domain.Core.Tenants;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
