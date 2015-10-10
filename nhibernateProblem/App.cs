using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Type;
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

            //add base data
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
                    Write(item);
                }

                Console.WriteLine("Fetch and filter using QueryOver...");

                DataItem dataItemAlias = null;
                var c = QueryOver.Of<DataItem>(() => dataItemAlias);

                c.WhereRestrictionOn(x => x.Source).IsInsensitiveLike("A");

                DataItem maxSendDateTimeAlias = null;

                /*     var subQuery = QueryOver.Of<DataItem>(() => maxSendDateTimeAlias)
                         .Select(Projections.ProjectionList()
                         .Add(Projections.Max(() => maxSendDateTimeAlias.SendDateTime))
                         .Add(Projections.Group(() => maxSendDateTimeAlias.Target)))
                         .Where(() => dataItemAlias.Source == maxSendDateTimeAlias.Source);
                         */
                var subQuery =
    QueryOver.Of<DataItem>()
        .Select(
            Projections.ProjectionList()
                .Add(Projections.SqlGroupProjection("max(SendDateTime) as maxSendDateTimeAlias", "Target",
                    new string[] { "maxAlias" }, new IType[] { NHibernate.NHibernateUtil.Int32 })));


                c.WithSubquery.WhereProperty(p => p.SendDateTime).In(subQuery);
                var result = c.GetExecutableQueryOver(Session).List<DataItem>();
                foreach (var item in result)
                {
                    Write(item);
                }

            }

            Console.WriteLine("Press D to empty data or other to close");
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.D)
                Session.CreateQuery("delete from DataItem").ExecuteUpdate();
        }

        private void Write(DataItem item)
        {
            Console.WriteLine("Id:{0}, source: {1} target: {2} version: {3} date: {4}", item.ID, item.Source, item.Target, item.Version, item.SendDateTime);
        }
    }
}