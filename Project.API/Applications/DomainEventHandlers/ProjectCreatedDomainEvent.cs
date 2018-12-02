using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using MediatR;
using Project.API.Applications.IntegrationEvents;
using Project.Domain.Events;

namespace Project.API.Applications.DomainEventHandlers
{
    public class ProjectCreatedDomainEvent:INotificationHandler<ProjectCreatedEvent>
    {
        private readonly ICapPublisher _capPublisher;

        public ProjectCreatedDomainEvent(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
        {
           var projectCreatedIntegrationEvent = new ProjectCreatedIntegrationEvent()
           {
               ProjectId = notification.Project.Id,
               Company = notification.Project.Company,
               Finstage = notification.Project.FinStage,
               Introduction = notification.Project.Introduction,
               ProjectAvatar = notification.Project.Avator,
               Tags = notification.Project.Tags,
               CreateTime = DateTime.Now,
               UserId = notification.Project.UserId
           };
           await _capPublisher.PublishAsync("pomelobutter.projectCreate", projectCreatedIntegrationEvent);
            
        }
    }
}