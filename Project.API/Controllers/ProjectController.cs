using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.API.Applications.Commands;
using Project.Domain.AggregatesModel;

namespace Project.API.Controllers
{
    [Route("api/[project]")]
    public class ProjectController : BaseController
    {
        private readonly IMediator _mediator;

        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] Domain.AggregatesModel.Project project)
        {
            var comand = new CreateProjectCommand {Project = project};
            var returnProject = await _mediator.Send(comand);
            return Ok(returnProject);
        }

        [HttpPut]
        [Route("view/{projectId}")]
        public async Task<IActionResult> ViewProject(int projectId)
        {
            var comand = new ViewProjectCommand
            {
                UserId = UserIdentity.UserId,
                UserName = UserIdentity.Name,
                Avatar = UserIdentity.Avatar,
                ProjectId = projectId
            };
            await _mediator.Send(comand);
            return Ok();
        }

        [HttpPut]
        [Route("join/{projectId}")]
        public async Task<IActionResult> Join([FromBody] ProjectContributor projectContributor)
        {
            var command = new JoinProjectCommand {ProjectContributor = projectContributor};
            await _mediator.Send(command);
            return Ok();
        }
    }
}