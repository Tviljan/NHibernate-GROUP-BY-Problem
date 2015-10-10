using FluentNHibernate.Mapping;
using nhibernateProblem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nhibernateProblem.Mappings
{
    public class DataItemMap : ClassMap<DataItem>
    {
        public DataItemMap()
        {
            Id(x => x.ID);
            Map(x => x.Source);
            Map(x => x.Target);
            Map(x => x.SendDateTime);
            Map(x => x.Version);
        }
    }
}
