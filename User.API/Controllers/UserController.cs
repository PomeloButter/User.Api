using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using User.API.Data;
using User.API.Models;

namespace User.API.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserContext _userContext;

        public UserController(UserContext userContext, ILogger<UserController> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user =await _userContext.AppUsers
                .AsTracking()
                .Include(u => u.Properties)
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            if (user == null) throw new UserOperationExpetion($"错误的用户上下文Id {UserIdentity.UserId} ");
            return Json(user);
        }
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]JsonPatchDocument<AppUser> jsonPatchDocument)
        {                  
            var user = await _userContext.AppUsers             
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);

            jsonPatchDocument.ApplyTo(user);

            foreach (var property in user.Properties)
            {
                _userContext.Entry(property).State = EntityState.Detached;
            }

            var originProperties =await _userContext.UserProperties.AsNoTracking().Where(u=>u.AppUserId==UserIdentity.UserId).ToListAsync();
            var allProperties = originProperties.Union(user.Properties).Distinct();

            var removeProperties = originProperties.Except(user.Properties);
            var newProperties = allProperties.Except(originProperties);

            foreach (var property in removeProperties)
            {
                _userContext.Remove(property);
            }
            foreach (var property in newProperties)
            {
                _userContext.Add(property);
            }

            _userContext.Update(user);
            _userContext.SaveChanges();
            return Json(user);
        }

        [HttpGet]
        [Route("baseinfo/{userId}")]
        public async Task<IActionResult> GetBaseInfo(int userId)
        {
            var user = await _userContext.AppUsers.SingleOrDefaultAsync(u => u.Id == userId);
            if (user==null)
            {
                return NotFound();
            }

            return Ok(new
            {
               userId= user.Id,
                user.Name,
                user.Company,
                user.Title,
                user.Avatar

            });
        }

        [Route("check-or-create")]
        [HttpPost]
        public async Task<IActionResult> CheckOrCreate(string phone)
        {
            var user =await _userContext.AppUsers.SingleOrDefaultAsync(u => u.Phone == phone);
            if (user==null)
            {
                user = new AppUser() {Phone = phone};
                _userContext.AppUsers.Add(user);
                await _userContext.SaveChangesAsync();
            }
            return Ok(new
            {
                user.Id,
                user.Name,
                user.Company,
                user.Title,
                user.Avatar
            });
        }

        [Route("tags")]
        [HttpGet]
        public async Task<IActionResult> GetUserTags()
        {
          return Ok(await _userContext.UserTags.Where(u => u.UserId == UserIdentity.UserId).ToListAsync());
        }

        [Route("search")]
        [HttpPost]
        public async Task<IActionResult> Search(string phone)
        {
            return Ok( await _userContext.AppUsers.Include(u => u.Properties)
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId));
        }

        [Route("tags")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserTags([FromBody]List<string> tags)
        {
            var originTags = await _userContext.UserTags.Where(u => u.UserId == UserIdentity.UserId).ToListAsync();
            var newTags = tags.Except(originTags.Select(t=>t.Tag));

            await _userContext.UserTags.AddRangeAsync(newTags.Select(t => new UserTag()
            {
                CreateTime = DateTime.Now,
                Tag = t,
                UserId = UserIdentity.UserId
            }));
            await _userContext.SaveChangesAsync();
            return Ok();
        }

    }
}