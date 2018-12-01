using MediatR;
using Project.Domain.AggregatesModel;

namespace Project.Domain.Events
{
    public class ProjectJoinedEvent : INotification
    {
        public ProjectContributor ProjectContributor { get; set; }
        public string Company { get; set; }
        public string Introduction { get; set; }
        public string Avatar { get; set; }

    }
}