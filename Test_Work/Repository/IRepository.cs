using System.Collections.Generic;
using System.Linq;

using Test_Work.Models;

namespace Test_Work.Repository
{
    public interface IRepository<TEntity> where TEntity : DbEntity
    {
        IList<TEntity> GetAll();
        IQueryable<TEntity> GetQuery();
        TEntity GetById(int id);
        int RowCount();
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
