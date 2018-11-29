using System.Threading.Tasks;
using Project.Domain.SeedWork;

namespace Project.Domain.AggregatesModel
{
    public interface IProjectRepository: IRepository<Project>
    {
        Task<Project> AddAsync(Project project);
        Task<Project> UpdateAsync(Project project);
        Task<Project> GetAsync(int id);
    }
}