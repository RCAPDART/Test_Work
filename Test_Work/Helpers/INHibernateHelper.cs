using NHibernate;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_Work.Helpers
{
    // Why you need interface for helper for NHibernate? How it can be replaced? It is already hard linked by name with NHibernate. At least it should be IOrmHelper or IDbHelper.
    public interface INHibernateHelper
    {
        ISessionFactory SessionFactory { get; }
        ISession OpenSession();
        void InitTable<T>() where T : class;
    }
}
