using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Domain.SeedWork;

namespace Project.Infrastructure
{
    public class ProjectContext:DbContext,IUnitOfWork
    {
        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new System.NotImplementedException();
        }
    }
}