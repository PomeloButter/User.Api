using System;

namespace Recommend.API.IntegrationEvents
{
    public class ProjectCreatedIntegrationEvent
    {

        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string ProjectAvatar { get; set; }
        public string Company { get; set; }
        public string Introduction { get; set; }
        public string Tags { get; set; }
        public string Finstage { get; set; }
        public DateTime CreateTime { get; set; }
    }
}