using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookForge.Domain.Entities;

namespace WebhookForge.Infrastructure.Data.Configurations;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("WorkspaceMembers");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Role)     .IsRequired().HasMaxLength(20).HasConversion<string>();
        builder.Property(m => m.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(m => new { m.WorkspaceId, m.UserId })
               .IsUnique()
               .HasDatabaseName("UX_WorkspaceMembers_WorkspaceUser");

        builder.HasOne(m => m.Workspace)
               .WithMany(w => w.Members)
               .HasForeignKey(m => m.WorkspaceId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_WorkspaceMembers_Workspaces");

        builder.HasOne(m => m.User)
               .WithMany(u => u.WorkspaceMemberships)
               .HasForeignKey(m => m.UserId)
               .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("FK_WorkspaceMembers_Users");
    }
}
