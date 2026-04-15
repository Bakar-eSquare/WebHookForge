using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookForge.Domain.Entities;

namespace WebhookForge.Infrastructure.Data.Configurations;

public class IncomingRequestConfiguration : IEntityTypeConfiguration<IncomingRequest>
{
    public void Configure(EntityTypeBuilder<IncomingRequest> builder)
    {
        builder.ToTable("IncomingRequests");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Method)     .IsRequired().HasMaxLength(10);
        builder.Property(r => r.ContentType).HasMaxLength(200);
        builder.Property(r => r.IpAddress)  .HasMaxLength(45);
        builder.Property(r => r.ReceivedAt) .IsRequired().HasDefaultValueSql("GETUTCDATE()");

        // Most-queried: list requests per endpoint ordered by date
        builder.HasIndex(r => new { r.EndpointId, r.ReceivedAt })
               .HasDatabaseName("IX_IncomingRequests_EndpointId_ReceivedAt");

        builder.HasOne(r => r.Endpoint)
               .WithMany(e => e.Requests)
               .HasForeignKey(r => r.EndpointId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_IncomingRequests_Endpoints");
    }
}
