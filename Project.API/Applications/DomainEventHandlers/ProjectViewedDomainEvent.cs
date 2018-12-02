using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;
using Project.API.Applications.IntegrationEvents;
using Project.Domain.Events;

namespace Project.API.Applications.DomainEventHandlers
{
    public class ProjectViewedDomainEvent: INotificationHandler<ProjectViewedEvent>
    {
        private readonly ICapPublisher _capPublisher;

        public ProjectViewedDomainEvent(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }
        public Task Handle(ProjectViewedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectViewedIntegrationEvent()
            {
                Company = notification.Company,
                Introduction = notification.Introduction,
                ProjectViewer = notification.ProjectViewer,
            };
            _capPublisher.Publish("finbook.projectapi.projectviewed", @event);
            return Task.CompletedTask;
        }
    }
}   