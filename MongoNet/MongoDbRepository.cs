﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization;
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

namespace MongoNet
{
    public class MongoDbRepository<TEntity, TMongoConnection> : IRepository<TEntity>
        where TEntity : EntityBase
        where TMongoConnection : IMongoConnection
    {
        private IMongoDatabase database;
        private IMongoCollection<TEntity> collection;
        private IMongoConnection mongoConnection;

        public IMongoCollection<TEntity> CollectionContext
        {
            get
            {
                return collection;
            }
        }
        public IMongoDatabase DatabaseContext
        {
            get
            {
                return database;
            }
        }

        public MongoDbRepository()
        {
            List<string> _mongoConnectionTypeName = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                                                   .Where(x => typeof(TMongoConnection).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                                                   .Select(x => x.AssemblyQualifiedName).ToList();
            Type _mongoConnectionType = Type.GetType(_mongoConnectionTypeName.FirstOrDefault());

            mongoConnection = ((IMongoConnection)Activator.CreateInstance(_mongoConnectionType));


            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(BsonType.String));

            if (!BsonClassMap.IsClassMapRegistered(typeof(TEntity)))
            { 
                BsonClassMap.RegisterClassMap<TEntity>(x =>
                {
                    x.AutoMap();
                    x.SetIgnoreExtraElements(true); 
                });
            }

          
           

            GetDatabase();
            GetCollection();
        }

        public TEntity Insert(TEntity entity)
        {
            entity.Id = ObjectId.GenerateNewId();
            entity.CreateDate = DateTime.Now;
            entity.UpdateDate = DateTime.Now;
            collection.InsertOne(entity);

            return entity;
        }
        public void Insert(TEntity entity, Action<TEntity> callback)
        {
            entity.Id = ObjectId.GenerateNewId();
            entity.CreateDate = DateTime.Now;
            entity.UpdateDate = DateTime.Now;
            collection.InsertOne(entity);

            callback(entity);
        }

        public ReplaceOneResult Update(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
            entity.UpdateDate = DateTime.Now;
            ReplaceOneResult result = collection.ReplaceOne(filter, entity);
            return result;
        }
        public ReplaceOneResult Update(TEntity entity, Action<TEntity> callback)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, entity.Id);
            entity.UpdateDate = DateTime.Now;
            ReplaceOneResult result = collection.ReplaceOne(filter, entity);

            callback(entity);

            return result;
        }

        public DeleteResult Delete(string Id)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, ObjectId.Parse(Id));
            DeleteResult result = collection.DeleteOne(filter);

            return result;
        }
        public DeleteResult Delete(string Id, Action<DeleteResult> callback)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, ObjectId.Parse(Id));
            DeleteResult result = collection.DeleteOne(filter);

            callback(result);

            return result;
        }

        public IMongoQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return collection
                .AsQueryable<TEntity>()
                    .Where(predicate);
        }
        public IMongoQueryable<TEntity> Select()
        {
            return collection.AsQueryable();
        }
        public IMongoQueryable<TEntity> Paging(int _skip, int _take)
        {
            return collection.AsQueryable().Skip(_skip).Take(_take);
        }

        public TEntity GetById(string id)
        {
            var filter = Builders<TEntity>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
            return collection.Find(filter).ToList().First();
        }
        public int Count()
        {
            return collection.AsQueryable().Count();
        }

        #region Helper Methods
        private void GetDatabase()
        {
            var client = new MongoClient(GetConnectionString());
            database = client.GetDatabase(GetDatabaseName());
        }

        private string GetConnectionString()
        {
            return mongoConnection.GetConnectionString();
        }

        private string GetDatabaseName()
        {
            return mongoConnection.GetDatabaseName();
        }

        private void GetCollection()
        {
            collection = database
                .GetCollection<TEntity>(typeof(TEntity).Name);
        }
        #endregion
    }
}