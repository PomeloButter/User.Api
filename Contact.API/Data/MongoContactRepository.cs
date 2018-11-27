using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Dots;
using Contact.API.Models;
using MongoDB.Driver;

namespace Contact.API.Data
{
    public class MongoContactRepository : IContactRepository
    {
        private readonly ContactContext _context;

        public MongoContactRepository(ContactContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateContactInfoAsync(UserIdentity userInfo, CancellationToken cancellationToken)
        {
            var contactBook =
                (await _context.ContactBooks.FindAsync(c => c.UserId == userInfo.UserId, null, cancellationToken))
                .FirstOrDefault(cancellationToken);
            if (contactBook == null) return true;

            var contactIds = contactBook.Contacts.Select(c => c.UserId);
            var filter = Builders<ContactBook>.Filter.And
            (
                Builders<ContactBook>.Filter.In(c => c.UserId, contactIds),
                Builders<ContactBook>.Filter.ElemMatch(c => c.Contacts, c => c.UserId == userInfo.UserId)
            );

            var update = Builders<ContactBook>.Update
                .Set("Contacts.$.Name", userInfo.Name)
                .Set("Contacts.$.Avatar", userInfo.Avatar)
                .Set("Contacts.$.Company", userInfo.Company)
                .Set("Contacts.$.Title", userInfo.Title);
            var updateResult = _context.ContactBooks.UpdateMany(filter, update);
            return updateResult.MatchedCount == updateResult.ModifiedCount;
        }

        public async Task<bool> AddContactAsync(int contactId, UserIdentity baseUserInfo,
            CancellationToken cancellationToken)
        {
            if (_context.ContactBooks.CountDocuments(c => c.UserId == contactId) == 0)
                await _context.ContactBooks.InsertOneAsync(new ContactBook
                {
                    UserId = contactId
                }, cancellationToken: cancellationToken);
            var filter = Builders<ContactBook>.Filter.Eq(c => c.UserId, contactId);
            var update = Builders<ContactBook>.Update.AddToSet(c => c.Contacts, new Models.Contact
            {
                UserId = baseUserInfo.UserId,
                Avatar = baseUserInfo.Avatar,
                Company = baseUserInfo.Company,
                Name = baseUserInfo.Name,
                Title = baseUserInfo.Title
            });
            var result = await _context.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount && result.ModifiedCount == 1;
        }

        public async Task<List<Models.Contact>> GetContactAsync(int userId, CancellationToken cancellationToken)
        {
            var contactBook = (await _context.ContactBooks.FindAsync(c => c.UserId == userId, cancellationToken: cancellationToken)).FirstOrDefault();
            if (contactBook != null) return contactBook.Contacts;
            return new List<Models.Contact>();
        }

        public async Task<bool> GetTagContactAsync(int userId, int contactId, List<string> tags,
            CancellationToken cancellationToken)
        {
            var filter = Builders<ContactBook>.Filter.And
            (
                Builders<ContactBook>.Filter.Eq(c => c.UserId, userId),
                Builders<ContactBook>.Filter.Eq("Contacts.UserId", contactId)
            );
            var update = Builders<ContactBook>.Update.Set("Contacts.$.Tags", tags);
            var result = await _context.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);
            return result.MatchedCount == result.ModifiedCount && result.ModifiedCount == 1;
        }
    }
}