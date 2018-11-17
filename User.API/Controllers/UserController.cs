using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.API.Data;

namespace User.API.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly UserContext _userContext;

        public UserController(UserContext userContext)
        {
            _userContext = userContext;
        }
   
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
          var user= _userContext.AppUsers
              .AsTracking()
              .Include(u=>u.Properties)
              .SingleOrDefault(u => u.Id == UserIdentity.UserId);

            if (user == null) return BadRequest();
            return Json(user);
        }


    }
}
