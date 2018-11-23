using System.Threading.Tasks;
using Contact.API.Dots;

namespace Contact.API.Services
{
    public class UserService:IUserService
    {
        public async Task<BaseUserInfo> GetBaseUserInfoAsync(int userId)
        {
            return  new BaseUserInfo
            {
                UserId = 1,
                Avatar ="",
                Title = "PM",
                Company = "",              
            };
        }
    }
}