using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Recommend.API.Data;
using Recommend.API.IntegrationEvents;
using Recommend.API.Models;
using Recommend.API.Services;

namespace Recommend.API.IntegrationEventHandlers
{
    public class ProjectCreatedIntegrationEventHandler : ICapSubscribe
    {
        private readonly RecommendContext _context;
        private readonly IUserService _userService;
        private readonly IContactService _contactService;

        public ProjectCreatedIntegrationEventHandler(RecommendContext context,IUserService userService,IContactService contactService)
        {
            _context = context;
            _userService = userService;
            _contactService = contactService;
        }
        [CapSubscribe("finbook.projectapi.projectcreated")]
        public async Task CreatedRecommendFromProject(ProjectCreatedIntegrationEvent @event)
        {

            var fromUser = await _userService.GetBaseUserInfoAsync(@event.UserId);
            var contacts = await _contactService.GetContactsByUserId(@event.UserId);
            foreach (var contact in contacts)
            {
                var recommend = new ProjectRecommend()
                {
                    FromUserId = @event.UserId,
                    Company = @event.Company,
                    Tags = @event.Tags,
                    ProjectId = @event.ProjectId,
                    ProjectAvatar = @event.ProjectAvatar,
                    FinStage = @event.Finstage,
                    RecommendTime = DateTime.Now,
                    CreatedTime = @event.CreateTime,
                    Introduction = @event.Introduction,
                    RecommendType = EnumRecommendType.Friend,
                    FromUserAvatar = fromUser.Avatar,
                    FromUserName = fromUser.Name,
                    UserId = contact.UserId,
                    
                };
                _context.ProjectRecommends.Add(recommend);
            }
            _context.SaveChanges();
        }
    }
}