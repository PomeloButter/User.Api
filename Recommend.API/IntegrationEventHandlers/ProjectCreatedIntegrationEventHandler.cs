using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Recommend.API.Data;
using Recommend.API.IntegrationEvents;

namespace Recommend.API.IntegrationEventHandlers
{
    public class ProjectCreatedIntegrationEventHandler : ICapSubscribe
    {
        private readonly RecommendContext _context;

        public ProjectCreatedIntegrationEventHandler(RecommendContext context)
        {
            _context = context;
        }
        [CapSubscribe("finbook.projectapi.projectcreated")]
        public Task CreatedRecommendFromProject(ProjectCreatedIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}