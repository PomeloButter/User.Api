using Project.Domain.AggregatesModel;

namespace Project.API.Applications.IntegrationEvents
{
    public class ProjectViewedIntegrationEvent
    {
        public ProjectViewer ProjectViewer { get; set; }
        public string Company { get; set; }
        public string Introduction { get; set; }
        public string Avatar { get; set; }
    }
}