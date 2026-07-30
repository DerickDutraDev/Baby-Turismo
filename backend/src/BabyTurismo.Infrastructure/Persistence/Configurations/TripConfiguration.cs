using BabyTurismo.Domain.Operations.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BabyTurismo.Infrastructure.Persistence.Configurations;

internal sealed class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("Trips");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TenantId).IsRequired();
        builder.Property(t => t.OrganizationId).IsRequired();
        builder.Property(t => t.BusinessUnitId).IsRequired();
        builder.Property(t => t.DriverId).IsRequired();
        builder.Property(t => t.VehicleId).IsRequired();

        builder.Property(t => t.Origin)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Destination)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Status).IsRequired();

        builder.Property(t => t.ScheduledStartDate).IsRequired();
        builder.Property(t => t.ScheduledEndDate).IsRequired();

        builder.HasOne<BabyTurismo.Domain.Operations.Drivers.Driver>()
            .WithMany()
            .HasForeignKey(t => t.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<BabyTurismo.Domain.Fleet.Vehicles.Vehicle>()
            .WithMany()
            .HasForeignKey(t => t.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
