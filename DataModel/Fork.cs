using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus_Adventure_RPG.DataModel
{
    sealed class Fork : IEvent
    {
        public string DisplayText { get; set; }

        public string Question { get; set; }

        public List<ForkPossibility> Possibilities { get; set; } = new List<ForkPossibility>();
    }
}
