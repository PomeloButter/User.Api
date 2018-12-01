using System;

namespace Recommend.API.IntegrationEvents
{
    public class ProjectCreatedIntegrationEvent
    {

        public int Projectid { get; set; }
        public int UserId { get; set; }
        public DateTime CreateTime { get; set; }

    }
}