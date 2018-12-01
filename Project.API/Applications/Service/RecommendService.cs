using System.Threading.Tasks;

namespace Project.API.Applications.Service
{
    public class RecommendService:IRecommendService
    {
        public Task<bool> IsProjectInRecommend(int projectId, int userId)
        {
            return Task.FromResult(true);
        }
    }
}