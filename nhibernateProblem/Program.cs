using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nhibernateProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    App app = new App();
                    app.Init(session, transaction);
                    app.Run();
                }
            }

        }

        private static ISessionFactory CreateSessionFactory()
        {

            return Fluently.Configure()
   .Database(MsSqlConfiguration.MsSql2008
                  .ConnectionString(
                      @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=mydb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
                  .ShowSql()
    )
    .Mappings(m =>
      m.FluentMappings.AddFromAssemblyOf<Program>())
    .BuildSessionFactory();
        }
    }
}
