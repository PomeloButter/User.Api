using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Domain.AggregatesModel;
using Project.Domain.Exceptions;

namespace Project.API.Applications.Commands
{
    public class JoinProjectCommandHandler:IRequestHandler<JoinProjectCommand>
    {
        private readonly IProjectRepository _projectRepository;

        public JoinProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task Handle(JoinProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectContributor.Id);
            if (project==null)
            {
                throw new ProjectDomainException($"not found");
            }

            if (project.UserId==request.ProjectContributor.UserId)
            {
                throw new ProjectDomainException($"不能自己加入自己的项目");
            }
            project.AddContributor(request.ProjectContributor);
            await _projectRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}