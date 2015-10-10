using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nhibernateProblem.Entities
{
    public class DataItem
    {
        public virtual int ID { get; protected set; }
        public virtual string Source { get; set; }
        public virtual string Target { get; set; }
        public virtual DateTime SendDateTime { get; set; }
        public virtual int Version { get; set; }
    }
}
