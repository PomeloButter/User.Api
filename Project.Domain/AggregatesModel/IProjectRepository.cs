using System.Threading.Tasks;

namespace Project.Domain.AggregatesModel
{
    public interface IProjectRepository
    {
        Task<Project> AddAsync(Project project);
        Task<Project> UpdateAsync(Project project);
        Task<Project> GetAsync(int id);
    }
}