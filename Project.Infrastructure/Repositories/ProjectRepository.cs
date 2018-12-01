using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Domain.AggregatesModel;
using Project.Domain.SeedWork;
using ProjectEntity = Project.Domain.AggregatesModel.Project;

namespace Project.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectContext _context;

        public ProjectRepository(ProjectContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;


        public ProjectEntity Add(ProjectEntity project)
        {
            if (project.IsTransient())
            {
              return  _context.Add(project).Entity;
            }

            return project;
        }

        public void Update(ProjectEntity project)
        {
            _context.Update(project);
        }

        public async Task<ProjectEntity> GetAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.ProjectPropetries)
                .Include(p => p.ProjectViewers)
                .Include(p => p.ProjectContributors)
                .Include(p => p.ProjectVisableRule)
                .SingleOrDefaultAsync(p => p.Id == id);
        }
    }
}