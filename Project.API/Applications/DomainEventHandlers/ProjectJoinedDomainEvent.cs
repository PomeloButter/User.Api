using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;
using Project.Domain.Events;

namespace Project.API.Applications.DomainEventHandlers
{
    public class ProjectJoinedDomainEvent: INotificationHandler<ProjectJoinedEvent>
    {
        private readonly ICapPublisher _capPublisher;

        public ProjectJoinedDomainEvent(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }
        public Task Handle(ProjectJoinedEvent notification, CancellationToken cancellationToken)
        {
            var @event = new ProjectJoinedIntegrationEvent() 
            {
                Company = notification.Company,
                Introduction = notification.Introduction,
                ProjectContributor = notification.ProjectContributor,
            };
            _capPublisher.Publish("finbook.projectapi.projectjoined", @event);
            return Task.CompletedTask;
        }
    }
}