using Microsoft.AspNetCore.Mvc;
using User.API.Dots;

namespace User.API.Controllers
{
    public class BaseController : Controller
    {
        protected UserIdentity UserIdentity=>new UserIdentity{UserId = 1,Name = "pomelobutter"};
    }
}