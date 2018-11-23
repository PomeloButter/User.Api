using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Dots;

namespace Contact.API.Data
{
    public interface IContactRepository
    {
        /// <summary>
        /// 更新联系人信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> UpdateContactInfoAsync(BaseUserInfo userInfo, CancellationToken cancellationToken);

        Task<bool> AddContactAsync(int contactId,BaseUserInfo baseUserInfo, CancellationToken cancellationToken);
        Task<List<Models.Contact>> GetContactAsync(int userId, CancellationToken cancellationToken);
        Task<bool> GetTagContactAsync(int userId, int contactId, List<string> tags, CancellationToken cancellationToken);

    }
}