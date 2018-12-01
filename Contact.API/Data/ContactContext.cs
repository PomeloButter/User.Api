using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contact.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Contact.API.Data
{
    public class ContactContext
    {
        private readonly IMongoDatabase _database;

        public ContactContext(IOptionsSnapshot<AppSettings> setting)
        {
            var appSettings = setting.Value;
            var client=new MongoClient(appSettings.MongoConnectionString);
            _database = client.GetDatabase(appSettings.MongoContactDatabase);
        }

        private void CheckAndCreateCollection(string name)
        {
            var collectionList = _database.ListCollections().ToList();
            var collectionName = new List<string>();
            collectionList.ForEach(b=>collectionName.Add(b["name"].AsString));
            if (!collectionName.Contains(name))
            {
                _database.CreateCollection(name);
            }
        }

        public IMongoCollection<ContactBook> ContactBooks
        {
            get
            {
                CheckAndCreateCollection("ContactBooks");return _database.GetCollection<ContactBook>("ContactBooks");
            }
        }

        public IMongoCollection<ContactApplyRequest> ContactApplyRequests
        {
            get { CheckAndCreateCollection("ContactApplyRequests"); return _database.GetCollection<ContactApplyRequest>("ContactApplyRequests"); }
        }
    }
}
