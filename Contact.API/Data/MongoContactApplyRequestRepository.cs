using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Models;
using MongoDB.Driver;

namespace Contact.API.Data
{
    public class MongoContactApplyRequestRepository : IContactApplyRequestRepository
    {
        private readonly ContactContext _context;

        public MongoContactApplyRequestRepository(ContactContext context)
        {
            _context = context;
        }

        public async Task<bool> AddRequestAsync(ContactApplyRequest request, CancellationToken cancellationToken)
        {
            var filter =
                Builders<ContactApplyRequest>.Filter.Where(r =>
                    r.UserId == request.UserId && r.ApplierId == request.ApplierId);
            if (await _context.ContactApplyRequests.CountDocumentsAsync(filter, cancellationToken: cancellationToken) >
                0)
            {
                var update = Builders<ContactApplyRequest>.Update.Set(r => r.ApplyTime, DateTime.Now);
//                var options=new UpdateOptions(){IsUpsert = true};
                var result =
                    await _context.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
                return result.MatchedCount == result.ModifiedCount && result.MatchedCount == 1;
            }

            await _context.ContactApplyRequests.InsertOneAsync(request, null, cancellationToken);
            return true;
        }

        public async Task<List<ContactApplyRequest>> GetRequestListAsync(int userId,
            CancellationToken cancellationToken)
        {
            return (await _context.ContactApplyRequests.FindAsync(r => r.UserId == userId,
                cancellationToken: cancellationToken)).ToList(cancellationToken);
        }


        public async Task<bool> ApprovalAsync(int userId, int applierId, CancellationToken cancellationToken)
        {
            var filter =
                Builders<ContactApplyRequest>.Filter.Where(r =>
                    r.UserId == userId && r.ApplierId == applierId);
            var update = Builders<ContactApplyRequest>.Update.Set(r => r.Approvaled, 1)
                .Set(r => r.HandledTime, DateTime.Now);
            var result = await _context.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount && result.MatchedCount == 1;
        }
    }
}