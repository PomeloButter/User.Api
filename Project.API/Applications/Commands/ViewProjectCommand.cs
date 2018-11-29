using MediatR;

namespace Project.API.Applications.Commands
{
    public class ViewProjectCommand:IRequest
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
    }
}