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
            return Ok(user.Id);
        }
    }
}