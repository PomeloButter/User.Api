using System.Threading.Tasks;
using Contact.API.Models;

namespace Contact.API.Data
{
    public interface IContactApplyRequestRepository
    {
        /// <summary>
        /// 添加申请好友的请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
       Task<bool>  AddRequestAsync(ContactApplyRequest request);
        /// <summary>
        /// 通过好友请求
        /// </summary>
        /// <param name="applierId"></param>
        /// <returns></returns>
        Task<bool> ApprovalAsync(int applierId);
        /// <summary>
        /// 获取好友申请列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
       Task<bool> GetRequestListAsync(int userId);
    }
}