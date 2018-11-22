using Contact.API.Dots;
using Microsoft.AspNetCore.Mvc;

namespace Contact.API.Controllers
{
    public class BaseController : Controller
    {
        protected UserIdentity UserIdentity=>new UserIdentity{UserId = 1};
    }
}