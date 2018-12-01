using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Project.Domain.AggregatesModel;
using Project.Domain.Exceptions;

namespace Project.API.Applications.Commands
{
    public class ViewProjectCommandHandler:IRequestHandler<ViewProjectCommand>
    {
        private readonly IProjectRepository _projectRepository;

        public ViewProjectCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task Handle(ViewProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetAsync(request.ProjectId);
            if (project == null)
            {
                throw new ProjectDomainException();
            }
            project.AddViewer(request.UserId,request.UserName,request.Avatar);
            await _projectRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}