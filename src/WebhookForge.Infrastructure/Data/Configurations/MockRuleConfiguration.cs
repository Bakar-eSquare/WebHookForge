using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebhookForge.Domain.Entities;

namespace WebhookForge.Infrastructure.Data.Configurations;

public class MockRuleConfiguration : IEntityTypeConfiguration<MockRule>
{
    public void Configure(EntityTypeBuilder<MockRule> builder)
    {
        builder.ToTable("MockRules");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)                .IsRequired().HasMaxLength(100);
        builder.Property(r => r.MatchMethod)         .HasMaxLength(10);
        builder.Property(r => r.MatchPath)           .HasMaxLength(500);
        builder.Property(r => r.MatchBodyExpression) .HasMaxLength(1000);
        builder.Property(r => r.ResponseStatus)      .IsRequired().HasDefaultValue((short)200);
        builder.Property(r => r.IsActive)            .IsRequired().HasDefaultValue(true);
        builder.Property(r => r.CreatedAt)           .IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(r => new { r.EndpointId, r.Priority, r.IsActive })
               .HasDatabaseName("IX_MockRules_EndpointId_Priority");

        builder.HasOne(r => r.Endpoint)
               .WithMany(e => e.MockRules)
               .HasForeignKey(r => r.EndpointId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_MockRules_Endpoints");
    }
}
