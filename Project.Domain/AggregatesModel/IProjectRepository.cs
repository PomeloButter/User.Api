using System.Threading.Tasks;
using Project.Domain.SeedWork;

namespace Project.Domain.AggregatesModel
{
    public interface IProjectRepository: IRepository<Project>
    {
        Project Add(Project project);
        void Update(Project project);
        Task<Project> GetAsync(int id);
    }
}