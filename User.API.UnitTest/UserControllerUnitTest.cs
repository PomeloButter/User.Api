using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using User.API.Controllers;
using User.API.Data;
using User.API.Models;
using Xunit;

namespace User.API.UnitTest
{
    public class UserControllerUnitTest
    {

        private UserContext GetUserContext()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var  userContext=new UserContext(options);
            userContext.AppUsers.Add(new AppUser() {Id = 1, Name = "pomelobutter"});
            userContext.SaveChanges();
            return userContext;
        }

        [Fact]
        public  async Task Get_ReturnRigthUser_WithExpectedParameters()
        {
            var context = GetUserContext();
            var loggerMoq = new Mock<ILogger<UserController>>();
            var logger = loggerMoq.Object;
            var conteroller = new UserController(context, logger);
            var response= await conteroller.Get();
            Assert.IsType<JsonResult>(response);
        }
    }
}
