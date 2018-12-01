using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Project.Infrastructure.EntityConfiguration
{
    public class ProjectVisibleRuleConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.ProjectVisableRule>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ProjectVisableRule> builder)
        {
            builder.ToTable("ProjectVisableRule")
                .HasKey(p => p.Id);
        }
    }
}