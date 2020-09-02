using NHibernate;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test_Work.Helpers
{
    public interface INHibernateHelper
    {
        ISessionFactory SessionFactory { get; }
        ISession OpenSession();
        void InitTable<T>() where T : class;
    }
}
