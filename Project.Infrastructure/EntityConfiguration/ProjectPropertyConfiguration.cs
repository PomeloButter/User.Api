using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.AggregatesModel;

namespace Project.Infrastructure.EntityConfiguration
{
    public class ProjectPropertyConfiguration : IEntityTypeConfiguration<ProjectPropetry>
    {
        public void Configure(EntityTypeBuilder<ProjectPropetry> builder)
        {
            builder.ToTable("ProjectPropetry").Property(p => p.Key).HasMaxLength(100);
            builder.HasKey(p => new {p.ProjectId, p.Key, p.Value});
            builder.Property(p => p.Value).HasMaxLength(100);
        }
    }
}