using BabyTurismo.Domain.Core.Tenants;
using BabyTurismo.Domain.Core.Users;
using BabyTurismo.Domain.Fleet.Vehicles;
using BabyTurismo.Domain.Operations.Drivers;
using BabyTurismo.Domain.Common;
using BabyTurismo.Domain.Common.Interfaces;
using BabyTurismo.Infrastructure.Persistence.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BabyTurismo.Infrastructure.Persistence;

/// <summary>
/// Main EF Core DbContext for BabyTurismo.
/// Applies Global Query Filters for multi-tenant isolation and soft delete.
/// </summary>
public sealed class FleetOsDbContext : DbContext, IUnitOfWork
{
    private readonly AuditInterceptor _auditInterceptor;
    private readonly IPublisher _publisher;
    private readonly ILogger<FleetOsDbContext> _logger;
    private Guid _currentTenantId;

    public FleetOsDbContext(
        DbContextOptions<FleetOsDbContext> options,
        AuditInterceptor auditInterceptor,
        IPublisher publisher,
        ILogger<FleetOsDbContext> logger)
        : base(options)
    {
        _auditInterceptor = auditInterceptor;
        _publisher = publisher;
        _logger = logger;
    }

    // ─── DbSets ───────────────────────────────────────────────────────

    // Core
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<BusinessUnit> BusinessUnits => Set<BusinessUnit>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Operations
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<BabyTurismo.Domain.Operations.Trips.Trip> Trips => Set<BabyTurismo.Domain.Operations.Trips.Trip>();

    // Fleet
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<BabyTurismo.Domain.Fleet.Fuel.FuelLog> FuelLogs => Set<BabyTurismo.Domain.Fleet.Fuel.FuelLog>();
    public DbSet<BabyTurismo.Domain.Fleet.Maintenance.MaintenanceRecord> Maintenances => Set<BabyTurismo.Domain.Fleet.Maintenance.MaintenanceRecord>();

    // Inventory
    public DbSet<BabyTurismo.Domain.Inventory.ProductCategory> ProductCategories => Set<BabyTurismo.Domain.Inventory.ProductCategory>();
    public DbSet<BabyTurismo.Domain.Inventory.Product> Products => Set<BabyTurismo.Domain.Inventory.Product>();
    public DbSet<BabyTurismo.Domain.Inventory.StockBalance> StockBalances => Set<BabyTurismo.Domain.Inventory.StockBalance>();
    public DbSet<BabyTurismo.Domain.Inventory.InventoryMovement> InventoryMovements => Set<BabyTurismo.Domain.Inventory.InventoryMovement>();

    // Finance
    public DbSet<BabyTurismo.Domain.Finance.CostCenter> CostCenters => Set<BabyTurismo.Domain.Finance.CostCenter>();
    public DbSet<BabyTurismo.Domain.Finance.FinancialCategory> FinancialCategories => Set<BabyTurismo.Domain.Finance.FinancialCategory>();
    public DbSet<BabyTurismo.Domain.Finance.FinancialMonth> FinancialMonths => Set<BabyTurismo.Domain.Finance.FinancialMonth>();
    public DbSet<BabyTurismo.Domain.Finance.FinancialTransaction> FinancialTransactions => Set<BabyTurismo.Domain.Finance.FinancialTransaction>();

    // Notifications & Issues
    public DbSet<BabyTurismo.Domain.Common.Notifications.Notification> Notifications => Set<BabyTurismo.Domain.Common.Notifications.Notification>();
    public DbSet<BabyTurismo.Domain.Fleet.VehicleIssues.VehicleIssueReport> VehicleIssueReports => Set<BabyTurismo.Domain.Fleet.VehicleIssues.VehicleIssueReport>();

    // Checklists
    public DbSet<BabyTurismo.Domain.Operations.Checklists.ChecklistItem> ChecklistItems => Set<BabyTurismo.Domain.Operations.Checklists.ChecklistItem>();
    public DbSet<BabyTurismo.Domain.Operations.Checklists.DailyChecklist> DailyChecklists => Set<BabyTurismo.Domain.Operations.Checklists.DailyChecklist>();

    // ─── Configuration ────────────────────────────────────────────────
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FleetOsDbContext).Assembly);

        // ── Global Query Filters ──────────────────────────────────────
        modelBuilder.Entity<Tenant>()
            .HasQueryFilter(t => t.DeletedAt == null);

        // All multi-tenant entities
        modelBuilder.Entity<Organization>()
            .HasQueryFilter(o => o.DeletedAt == null && o.TenantId == _currentTenantId);

        modelBuilder.Entity<BusinessUnit>()
            .HasQueryFilter(bu => bu.DeletedAt == null && bu.TenantId == _currentTenantId);

        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.DeletedAt == null && u.TenantId == _currentTenantId);

        modelBuilder.Entity<Driver>()
            .HasQueryFilter(d => d.DeletedAt == null && d.TenantId == _currentTenantId);

        modelBuilder.Entity<Vehicle>()
            .HasQueryFilter(v => v.DeletedAt == null && v.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Operations.Trips.Trip>()
            .HasQueryFilter(t => t.DeletedAt == null && t.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Fleet.Fuel.FuelLog>()
            .HasQueryFilter(f => f.DeletedAt == null && f.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Fleet.Maintenance.MaintenanceRecord>()
            .HasQueryFilter(m => m.DeletedAt == null && m.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Inventory.ProductCategory>()
            .HasQueryFilter(c => c.DeletedAt == null && c.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Inventory.Product>()
            .HasQueryFilter(p => p.DeletedAt == null && p.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Inventory.StockBalance>()
            .HasQueryFilter(s => s.DeletedAt == null && s.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Inventory.InventoryMovement>()
            .HasQueryFilter(m => m.DeletedAt == null && m.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Finance.CostCenter>()
            .HasQueryFilter(c => c.DeletedAt == null && c.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Finance.FinancialCategory>()
            .HasQueryFilter(c => c.DeletedAt == null && c.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Finance.FinancialMonth>()
            .HasQueryFilter(m => m.DeletedAt == null && m.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Finance.FinancialTransaction>()
            .HasQueryFilter(t => t.DeletedAt == null && t.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Common.Notifications.Notification>()
            .HasQueryFilter(n => n.DeletedAt == null && n.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Fleet.VehicleIssues.VehicleIssueReport>()
            .HasQueryFilter(i => i.DeletedAt == null && i.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Operations.Checklists.ChecklistItem>()
            .HasQueryFilter(c => c.DeletedAt == null && c.TenantId == _currentTenantId);

        modelBuilder.Entity<BabyTurismo.Domain.Operations.Checklists.DailyChecklist>()
            .HasQueryFilter(c => c.DeletedAt == null && c.TenantId == _currentTenantId);

        base.OnModelCreating(modelBuilder);
    }

    public void SetTenantId(Guid tenantId, Guid? userId = null)
    {
        _currentTenantId = tenantId;
    }

    // ─── Domain Events ────────────────────────────────────────────────
    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(x => x.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    // ─── IUnitOfWork Implementation ───────────────────────────────────
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CommitAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CommitAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default)
    {
        _currentTenantId = tenantId;
        return await SaveChangesAsync(cancellationToken);
    }
}
