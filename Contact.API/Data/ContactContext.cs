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
        private IMongoDatabase _database;
        private readonly IMongoCollection<ContactBook> _collection;
        private readonly AppSettings _appSettings;

        public ContactContext(IOptionsSnapshot<AppSettings> setting)
        {
            _appSettings = setting.Value;
            var client=new MongoClient(_appSettings.MongoConnectionString);
            if (client!=null)
            {
                _database = client.GetDatabase(_appSettings.MongoContactDatabase);
            }
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
