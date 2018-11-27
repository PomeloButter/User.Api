using System.Threading.Tasks;
using Contact.API.Dots;

namespace Contact.API.Services
{
    public interface IUserService
    {
        Task<UserIdentity> GetBaseUserInfoAsync(int userId);
    }
}