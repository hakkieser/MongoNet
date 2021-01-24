using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.MongoNet
{
    public interface IRepository<TEntity>
    {
        TEntity Insert(TEntity entity);
        ReplaceOneResult Update(TEntity entity, DateTime? _updateDate);
        DeleteResult Delete(string Id);
        IMongoQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IMongoQueryable<TEntity> Select();
        IMongoQueryable<TEntity> Paging(int skip, int take);
        TEntity GetById(string id);
        int Count();
    }
}
