using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookForge.Domain.Entities;

namespace WebhookForge.Infrastructure.Data.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)     .IsRequired().HasMaxLength(100);
        builder.Property(w => w.Slug)     .IsRequired().HasMaxLength(100);
        builder.Property(w => w.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(w => w.Slug).IsUnique().HasDatabaseName("UX_Workspaces_Slug");

        builder.HasIndex(w => w.OwnerId).HasDatabaseName("IX_Workspaces_OwnerId");

        builder.HasOne(w => w.Owner)
               .WithMany(u => u.OwnedWorkspaces)
               .HasForeignKey(w => w.OwnerId)
               .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("FK_Workspaces_Users");
    }
}
