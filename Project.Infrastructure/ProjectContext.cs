using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Project.Domain.SeedWork;
using Project.Infrastructure.EntityConfiguration;

namespace Project.Infrastructure
{
    public class ProjectContext:DbContext,IUnitOfWork
    {
        private readonly IMediator _mediator;

        public ProjectContext(DbContextOptions<ProjectContext> options,IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }
        public DbSet<Domain.AggregatesModel.Project> Projects { get; set; }
        public DbSet<Domain.AggregatesModel.ProjectContributor> ProjectContributors { get; set; }
        public DbSet<Domain.AggregatesModel.ProjectViewer> ProjectViewers { get; set; }
        public DbSet<Domain.AggregatesModel.ProjectPropetry> ProjectPropetries { get; set; }
        public DbSet<Domain.AggregatesModel.ProjectVisableRule> ProjectVisableRules { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProjectEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectContributorConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectViewConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectPropertyConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectVisibleRuleConfiguration());
        }
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {

            await _mediator.DispatchDomainEventsAsync(this);
            await base.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}