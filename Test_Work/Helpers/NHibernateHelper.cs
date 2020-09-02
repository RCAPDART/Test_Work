using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;

using Org.BouncyCastle.Asn1.X509.Qualified;

using Test_Work.Models;

namespace Test_Work.Helpers
{
    public class NHibernateHelper : INHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static NHibernate.Cfg.Configuration cfg;

        public ISessionFactory SessionFactory
        {
            get { return _sessionFactory ?? (_sessionFactory = CreateSessionFactory()); }
        }

        public ISession OpenSession()
        {
            var session = SessionFactory.OpenSession();

            return session;
        }

        public void InitTable<T>() where T : class
        {
            using (var session = this.OpenSession())
            {
                try
                {
                    var t = session.QueryOver<T>().RowCount();
                }
                catch (HibernateException e)
                {
                    var mySqlException = (MySql.Data.MySqlClient.MySqlException)e.InnerException; 
                    if (mySqlException.Number == 1146)
                    {
                        new SchemaExport(cfg).Execute(true, true, false);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private ISessionFactory CreateSessionFactory()
        {
            const string host = "localhost";
            const string port = "3307";
            const string database = "test_work";
            const string username = "root";
            const string password = "root";
            const string sslMode = "none";

            cfg = new Configuration();
            cfg.DataBaseIntegration(x =>
            {
                x.ConnectionString = $"server={host};" +
                                     $"port={port};" +
                                     $"database={database};" +
                                     $"user={username};" +
                                     $"password={password};" +
                                     $"sslmode={sslMode};";

                x.ConnectionProvider<DriverConnectionProvider>();
                x.Driver<MySqlDataDriver>();
                x.Dialect<MySQLDialect>();
                x.LogSqlInConsole = true;
            });

            cfg.AddAssembly(Assembly.GetExecutingAssembly());
            return cfg.BuildSessionFactory();



        }
    }
}
