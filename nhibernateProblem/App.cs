using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using nhibernateProblem.Entities;
using System;

namespace nhibernateProblem
{
    internal class App
    {
        public App()
        {
        }

        public ISession Session { get; private set; }
        public ITransaction Transaction { get; private set; }

        internal void Init(ISession session, ITransaction transaction)
        {

            this.Session = session;
            this.Transaction = transaction;

            Session.SaveOrUpdate(new DataItem { SendDateTime = DateTime.Now.AddDays(-2), Source = "A", Target = "B", Version = 1 });
            Session.SaveOrUpdate(new DataItem { SendDateTime = DateTime.Now, Source = "A", Target = "B", Version = 2 });
            Session.SaveOrUpdate(new DataItem { SendDateTime = DateTime.Now.AddDays(-2), Source = "A", Target = "C", Version = 1 });
            Session.SaveOrUpdate(new DataItem { SendDateTime = DateTime.Now, Source = "A", Target = "C", Version = 2 });
            Session.SaveOrUpdate(new DataItem { SendDateTime = DateTime.Now, Source = "A", Target = "D", Version = 1 });

            Transaction.Commit();
        }

        internal void Run()
        {
            using (Session.BeginTransaction())
            {

                Console.WriteLine("List all...");

                var items = Session.CreateCriteria(typeof(DataItem))
                  .List<DataItem>();

                foreach (var item in items)
                {
                    Console.WriteLine(item.ID);
                }

                Console.WriteLine("Fetch and filter using QueryOver...");

                DataItem dataItemAlias = null;
                var c = QueryOver.Of<DataItem>(() => dataItemAlias);

                c.WhereRestrictionOn(x => x.Source).IsInsensitiveLike("A");

                DataItem maxSendDateTimeAlias = null;

                var subQuery = QueryOver.Of<DataItem>(() => maxSendDateTimeAlias)
                    .Select(Projections.ProjectionList()
                    .Add(Projections.Max(() => maxSendDateTimeAlias.SendDateTime))
                    .Add(Projections.Group(() => maxSendDateTimeAlias.Target)))
                    .Where(() => dataItemAlias.Source == maxSendDateTimeAlias.Source);

                c.WithSubquery.WhereProperty(p => p.SendDateTime).In(subQuery);
                var result = c.GetExecutableQueryOver(Session).List<DataItem>();
                foreach (var item in result)
                {
                    Console.WriteLine(item.ID);
                }

            }

            Console.WriteLine("Press D to empty data or other to close");
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.D)
                Session.CreateQuery("delete from DataItem").ExecuteUpdate();
        }
    }
}