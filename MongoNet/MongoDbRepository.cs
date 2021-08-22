using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.MongoNet
{
    public class MongoDbRepository<TEntity, TMongoConnection>
        where TEntity : MongoDbRepository<TEntity, TMongoConnection>
        where TMongoConnection : IMongoConnection
    {


        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }


        static private readonly IMongoDatabase database;
        static private readonly IMongoCollection<TEntity> collection;
        static private readonly IMongoConnection mongoConnection;

        static public IMongoCollection<TEntity> CollectionContext
        {
            get
            {
                return collection;
            }
        }
        static public IMongoDatabase DatabaseContext
        {
            get
            {
                return database;
            }
        }

        static MongoDbRepository()
        {
            mongoConnection = ((IMongoConnection)Activator.CreateInstance(typeof(TMongoConnection)));

            var conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

            try
            {
                BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));
            }
            catch { }

            if (!BsonClassMap.IsClassMapRegistered(typeof(TEntity)))
            {
                BsonClassMap.RegisterClassMap<TEntity>(x =>
                {
                    x.SetIgnoreExtraElements(true);
                    x.AutoMap();
                });
            }

            database = GetDatabase();
            collection = GetCollection(database);
        }

        public TEntity Insert()
        {
            this.Id = ObjectId.GenerateNewId().ToString();
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;

            collection.InsertOne((TEntity)this);

            return (TEntity)this;
        }

        public ReplaceOneResult Update(DateTime? _updateDate = null)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, this.Id);

            if (_updateDate == null)
            {
                this.UpdateDate = DateTime.Now;
            }
            else
            {
                this.UpdateDate = (DateTime)_updateDate;
            }

            ReplaceOneResult result = collection.ReplaceOne(filter, (TEntity)this);
            return result;
        }


        #region Static Methods
        static public DeleteResult Delete(string Id)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, Id);
            DeleteResult result = collection.DeleteOne(filter);

            return result;
        }

        static public IMongoQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return collection
                .AsQueryable<TEntity>()
                    .Where(predicate);
        }
        static public IMongoQueryable<TEntity> Select()
        {
            return collection.AsQueryable();
        }
        static public IMongoQueryable<TEntity> Paging(int _skip, int _take)
        {
            return collection.AsQueryable().Skip(_skip).Take(_take);
        }

        static public TEntity GetById(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, id);
            return collection.Find(filter).ToList().First();
        }
        static public int Count()
        {
            return collection.AsQueryable().Count();
        }
        #endregion

        #region Helper Methods
       

        static private string GetConnectionString()
        {
            return mongoConnection.GetConnectionString();
        }

        static private string GetDatabaseName()
        {
            return mongoConnection.GetDatabaseName();
        }



        static private IMongoDatabase GetDatabase()
        {
            var client = new MongoClient(GetConnectionString());

            return client.GetDatabase(GetDatabaseName());
        }
        static private IMongoCollection<TEntity> GetCollection(IMongoDatabase _database)
        {
            return _database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        #endregion
    }
}