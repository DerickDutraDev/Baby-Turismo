using BabyTurismo.Domain.Common;

namespace BabyTurismo.Domain.Core.Users;

public sealed class RefreshToken : Entity
{
    private RefreshToken() { }

    public RefreshToken(
        Guid id,
        Guid userId,
        string token,
        DateTimeOffset expiresAt,
        Guid tenantId,
        Guid organizationId,
        Guid businessUnitId)
        : base(id, tenantId, organizationId, businessUnitId)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        IsRevoked = false;
    }

    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    public bool IsExpired => ExpiresAt < DateTimeOffset.UtcNow;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string? replacedByToken = null)
    {
        if (!IsRevoked)
        {
            IsRevoked = true;
            RevokedAt = DateTimeOffset.UtcNow;
            ReplacedByToken = replacedByToken;
        }
    }
}

