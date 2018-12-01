using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Recommend.API.Models;

namespace Recommend.API.Data
{
    public class RecommendContext : DbContext
    {
        public RecommendContext( DbContextOptions<RecommendContext> options) : base(options)
        {
        }

        public DbSet<ProjectRecommend> ProjectRecommends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectRecommend>().ToTable("ProjectRecommends").HasKey(p => p.Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}