using System;

namespace Project.API.Applications.DomainEventHandlers
{
    public class ProjectCreatedIntegrationEvent
    {

        public int Projectid { get; set; }
        public int UserId { get; set; }
        public DateTime CreateTime { get; set; }

    }
}