using System.Threading.Tasks;
using Project.Domain.AggregatesModel;
using Project.Domain.SeedWork;
using ProjectEntity=Project.Domain.AggregatesModel.Project;
namespace Project.Infrastructure.Repositories
{
    public class ProjectRepository:IProjectRepository
    {
        public IUnitOfWork UnitOfWork { get; }
        public Task<ProjectEntity> AddAsync(ProjectEntity project)
        {
            throw new System.NotImplementedException();
        }

        public Task<ProjectEntity> UpdateAsync(ProjectEntity project)
        {
            throw new System.NotImplementedException();
        }

        public Task<ProjectEntity> GetAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}