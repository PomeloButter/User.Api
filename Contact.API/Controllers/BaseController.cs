using System;
using System.Linq;
using Contact.API.Dots;
using Microsoft.AspNetCore.Mvc;

namespace Contact.API.Controllers
{
    public class BaseController : Controller
    {
        protected UserIdentity UserIdentity=>new UserIdentity
        {
            UserId = Convert.ToInt16(User.Claims.FirstOrDefault(c=>c.Type=="sub")?.Value),
            Name = User.Claims.FirstOrDefault(c=>c.Type=="name")?.Value,
            Company = User.Claims.FirstOrDefault(c=>c.Type=="company")?.Value,
            Title = User.Claims.FirstOrDefault(c=>c.Type=="title")?.Value,
            Avatar =User.Claims.FirstOrDefault(c=>c.Type=="avatar")?.Value
        };
    }
}