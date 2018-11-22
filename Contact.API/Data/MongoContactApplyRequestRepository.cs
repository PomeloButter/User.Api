using System.Threading.Tasks;
using Contact.API.Models;

namespace Contact.API.Data
{
    public class MongoContactApplyRequestRepository:IContactApplyRequestRepository
    {
        private readonly ContactContext _context;

        public MongoContactApplyRequestRepository(ContactContext  context)
        {
            _context = context;
        }

        public Task<bool> AddRequestAsync(ContactApplyRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ApprovalAsync(int applierId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> GetRequestListAsync(int userId)
        {
            throw new System.NotImplementedException();
        }
    }
}