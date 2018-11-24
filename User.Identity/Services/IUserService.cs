using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace User.Identity.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 检查手机是否注册，如果没有注册一个用户
        /// </summary>
        /// <param name="phone"></param>
        Task<Dtos.UserInfo> CheckOrCreate(string phone);
    }
}