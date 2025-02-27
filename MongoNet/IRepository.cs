using MongoDB.Driver; 
using System; 
using System.Linq;
using System.Linq.Expressions; 

namespace MongoDB.MongoNet
{
    public interface IRepository<TEntity>
    {
        TEntity Insert(TEntity entity);
        ReplaceOneResult Update(TEntity entity, DateTime? _updateDate);
        DeleteResult Delete(string Id);
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Select();
        IQueryable<TEntity> Paging(int skip, int take);
        TEntity GetById(string id);
        int Count();
    }
}
