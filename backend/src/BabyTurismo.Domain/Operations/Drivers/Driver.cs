using BabyTurismo.Domain.Common;
using BabyTurismo.Domain.Common.ValueObjects;
using BabyTurismo.Shared.Results;

namespace BabyTurismo.Domain.Operations.Drivers;

public sealed class Driver : AggregateRoot
{
    private Driver() { }

    private Driver(
        Guid id,
        Guid tenantId,
        Guid organizationId,
        Guid businessUnitId,
        Guid userId,
        string cnhNumber,
        string cnhCategory,
        DateTime cnhExpirationDate)
        : base(id, tenantId, organizationId, businessUnitId)
    {
        UserId            = userId;
        CnhNumber         = cnhNumber;
        CnhCategory       = cnhCategory;
        CnhExpirationDate = DateTime.SpecifyKind(cnhExpirationDate.Date, DateTimeKind.Utc);
        Status            = DriverStatus.Active;
        IsAvailable       = true;
        CreatedAt         = DateTimeOffset.UtcNow;
    }

    public Guid   UserId { get; private set; }
    public string CnhNumber         { get; private set; } = default!;
    public string CnhCategory       { get; private set; } = default!;
    public DateTime CnhExpirationDate { get; private set; }
    public DriverStatus Status      { get; private set; }

    public string? Phone    { get; private set; }
    public string? PhotoUrl { get; private set; }

    public bool   IsAvailable       { get; private set; }

    public static Result<Driver> Create(
        Guid     tenantId,
        Guid     organizationId,
        Guid     businessUnitId,
        Guid     userId,
        string   cnhNumber,
        string   cnhCategory,
        DateTime cnhExpirationDate,
        string?  phone = null)
    {
        if (string.IsNullOrWhiteSpace(cnhNumber))
            return Result.Failure<Driver>(Error.Validation("Driver.CnhRequired", "CNH is required."));

        if (string.IsNullOrWhiteSpace(cnhCategory))
            return Result.Failure<Driver>(Error.Validation("Driver.CnhCategoryRequired", "CNH category is required."));

        if (cnhExpirationDate.Date < DateTime.UtcNow.Date)
            return Result.Failure<Driver>(Error.Validation("Driver.CnhExpired", "Cannot register a driver with an expired CNH."));

        var driver = new Driver(
            Guid.NewGuid(), tenantId, organizationId, businessUnitId,
            userId, cnhNumber, cnhCategory, cnhExpirationDate)
        {
            Phone = phone
        };

        return Result.Success(driver);
    }

    public Result UpdateCnh(string cnhNumber, string cnhCategory, DateTime cnhExpirationDate)
    {
        if (string.IsNullOrWhiteSpace(cnhNumber))
            return Result.Failure(Error.Validation("Driver.CnhRequired", "CNH is required."));

        if (string.IsNullOrWhiteSpace(cnhCategory))
            return Result.Failure(Error.Validation("Driver.CnhCategoryRequired", "CNH category is required."));

        if (cnhExpirationDate.Date < DateTime.UtcNow.Date)
            return Result.Failure(Error.Validation("Driver.CnhExpired", "CNH expiration date cannot be in the past."));

        CnhNumber         = cnhNumber;
        CnhCategory       = cnhCategory;
        CnhExpirationDate = DateTime.SpecifyKind(cnhExpirationDate.Date, DateTimeKind.Utc);
        UpdatedAt         = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public void UpdateContact(string? phone, string? photoUrl)
    {
        Phone     = phone;
        PhotoUrl  = photoUrl;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateStatus(DriverStatus status)
    {
        Status    = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SetAvailability(bool available)
    {
        IsAvailable = available;
        UpdatedAt   = DateTimeOffset.UtcNow;
    }



    public bool HasValidCnh() => CnhExpirationDate.Date >= DateTime.UtcNow.Date;

    public bool IsCnhExpiringSoon(int warningDays = 30)
        => CnhExpirationDate.Date <= DateTime.UtcNow.Date.AddDays(warningDays);
}

public enum DriverStatus
{
    Active   = 0,
    Inactive = 1,
    OnLeave  = 2  // Afastado/Férias
}
