using BabyTurismo.Domain.Fleet.VehicleIssues;
using BabyTurismo.Domain.Fleet.Vehicles;
using BabyTurismo.Domain.Operations.Drivers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BabyTurismo.Infrastructure.Persistence.Configurations;

internal sealed class VehicleIssueReportConfiguration : IEntityTypeConfiguration<VehicleIssueReport>
{
    public void Configure(EntityTypeBuilder<VehicleIssueReport> builder)
    {
        builder.ToTable("VehicleIssueReports");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        
        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Driver>()
            .WithMany()
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.Status);
    }
}
