using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookForge.Domain.Entities;

namespace WebhookForge.Infrastructure.Data.Configurations;

public class WebhookEndpointConfiguration : IEntityTypeConfiguration<WebhookEndpoint>
{
    public void Configure(EntityTypeBuilder<WebhookEndpoint> builder)
    {
        builder.ToTable("Endpoints");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Token)      .IsRequired().HasMaxLength(64);
        builder.Property(e => e.Name)       .IsRequired().HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.IsActive)   .IsRequired().HasDefaultValue(true);
        builder.Property(e => e.CreatedAt)  .IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => e.Token)
               .IsUnique()
               .HasDatabaseName("UX_Endpoints_Token");

        builder.HasIndex(e => e.WorkspaceId)
               .HasDatabaseName("IX_Endpoints_WorkspaceId");

        builder.HasOne(e => e.Workspace)
               .WithMany(w => w.Endpoints)
               .HasForeignKey(e => e.WorkspaceId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_Endpoints_Workspaces");
    }
}
