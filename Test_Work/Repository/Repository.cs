using NHibernate;

using System.Collections.Generic;
using System.Linq;

using Test_Work.Helpers;
using Test_Work.Models;

namespace Test_Work.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : DbEntity
    {
        private readonly INHibernateHelper _nhibernateHelper;

        public Repository(INHibernateHelper nhibernateHelper)
        {
            _nhibernateHelper = nhibernateHelper;
        }

        public IList<TEntity> GetAll()
        {
            using (ISession session = _nhibernateHelper.OpenSession())
            {
                return session.Query<TEntity>().ToList();
            }
        }

        public IQueryable<TEntity> GetQuery()
        {
            using (ISession session = _nhibernateHelper.OpenSession())
            {
                return session.Query<TEntity>();
            }
        }

        public TEntity GetById(int id)
        {
            using (ISession session = _nhibernateHelper.OpenSession())
            {
                return session.Get<TEntity>(id);
            }
        }

        public int RowCount()
        {
            using (ISession session = _nhibernateHelper.OpenSession())
            {
                return session.QueryOver<TEntity>().RowCount(); ;
            }
        }

        public void Insert(TEntity entity)
        {
            using (ISession session = _nhibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(entity);
                    transaction.Commit();
                }
            }
        }

        public void Update(TEntity entity)
        {
            using (ISession session = _nhibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(entity);
                    transaction.Commit();
                }
            }
        }

        public void Delete(TEntity entity)
        {
            using (ISession session = _nhibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(entity);
                    transaction.Commit();
                }
            }
        }
    }
}
