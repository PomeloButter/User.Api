using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using User.API.Data;

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
            var user = _userContext.AppUsers
                .AsTracking()
                .Include(u => u.Properties)
                .SingleOrDefault(u => u.Id == UserIdentity.UserId);

            if (user == null) throw new UserOperationExpetion($"错误的用户上下文Id {UserIdentity.UserId} ");
            return Json(user);
        }
    }
}