using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.API.Applications.Commands;
using Project.API.Applications.Queries;
using Project.API.Applications.Service;
using Project.Domain.AggregatesModel;

namespace Project.API.Controllers
{
    [Route("api/projects")]
    public class ProjectController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IProjectQueries _projectQueries;
        private readonly IRecommendService _recommendService;

        public ProjectController(IMediator mediator, IRecommendService recommendService, IProjectQueries projectQueries)
        {
            _mediator = mediator;
            _recommendService = recommendService;
            _projectQueries = projectQueries;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetProjects()
        {
            var project = await _projectQueries.GetProjectByUserId(UserIdentity.UserId);
            return Ok(project);
        }
        [HttpGet]
        [Route("recommends/{projectId}")]
        public async Task<IActionResult> GetRecommendProjectDetail(int projectId)
        {
            if (await _recommendService.IsProjectInRecommend(projectId, UserIdentity.UserId))
                return BadRequest("无权限查看");
            var project = await _projectQueries.GetProjectDetail(projectId);
            if (project.UserId==UserIdentity.UserId)
            {
                return Ok(project);
            }
            return BadRequest("无权限查看"); 
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create([FromBody] Domain.AggregatesModel.Project project)
        {
            if (project==null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            project.UserId = UserIdentity.UserId;
            var comand = new CreateProjectCommand {Project = project};
            var returnProject = await _mediator.Send(comand);
            return Ok(returnProject);
        }

        [HttpPut]
        [Route("view/{projectId}")]
        public async Task<IActionResult> ViewProject(int projectId)
        {
            if (await _recommendService.IsProjectInRecommend(projectId, UserIdentity.UserId))
                return BadRequest("没有查看该项目的权限");
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