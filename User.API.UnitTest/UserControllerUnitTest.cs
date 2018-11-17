using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
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

        private (UserController,UserContext) GetUserController()
        {
            var context = GetUserContext();
            var loggerMoq = new Mock<ILogger<UserController>>();
            var logger = loggerMoq.Object;
            return (new UserController(context, logger),context);
            
        }

        [Fact]
        public  async Task Get_ReturnRigthUser_WithExpectedParameters()
        {
            (UserController userController, UserContext userContext) = GetUserController();
            var response= await userController.Get();
            var result = response.Should().BeOfType<JsonResult>().Subject;
            var appUser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appUser.Id.Should().Be(1);
            appUser.Name.Should().Be("pomelobutter");
        }
        [Fact]
        public async Task Patch_ReturnNewName_WithExpectedNewNameParameter()
        {
            (UserController userController, UserContext userContext) = GetUserController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(u => u.Name, "pomelo");
            var response = await userController.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;
            var appUser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appUser.Name.Should().Be("pomelo");
            var userModel = await userContext.AppUsers.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Should().NotBeNull();
            userModel.Name.Should().Be("pomelo");
        }
        [Fact]
        public async Task Patch_ReturnNewProperties_WithaddNewProperties()
        {
            (UserController userController, UserContext userContext) = GetUserController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(u => u.Properties, new List<UserProperty>()
            {
                new UserProperty()
                {
                    Key = "fin_industry",
                    Value = "»¥ÁªÍø",
                    Text = "»¥ÁªÍø"
                }
            });
            var response = await userController.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;
            var appUser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appUser.Properties.Count.Should().Be(1);
            appUser.Properties.First().Value.Should().Be("»¥ÁªÍø");
            appUser.Properties.First().Key.Should().Be("fin_industry");
            var userModel = await userContext.AppUsers.SingleOrDefaultAsync(u => u.Id == 1);

            userModel.Properties.Count.Should().Be(1);
            userModel.Properties.First().Value.Should().Be("»¥ÁªÍø");
            userModel.Properties.First().Key.Should().Be("fin_industry");
        }
        [Fact]
        public async Task Patch_ReturnNewProperties_WithRemoveProperties()
        {
            (UserController userController, UserContext userContext) = GetUserController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(u => u.Properties, new List<UserProperty>()
            {             
            });
            var response = await userController.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            var appUser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appUser.Properties.Should().BeEmpty();
           
            var userModel = await userContext.AppUsers.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Properties.Should().BeEmpty();         
        }
    }
}
