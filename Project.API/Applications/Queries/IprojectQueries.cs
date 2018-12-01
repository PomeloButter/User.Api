using System.Threading.Tasks;

namespace Project.API.Applications.Queries
{
    public interface IProjectQueries
    {
        Task<dynamic> GetProjectByUserId(int userId);
        Task<dynamic> GetProjectDetail(int projectId);
    }
}