using MediatR;
using  ProjectEntity=Project.Domain.AggregatesModel.Project;
namespace Project.Domain.Events
{
    public class ProjectCreatedEvent:INotification
    {
        public ProjectEntity Project { get; set; }
    }
}