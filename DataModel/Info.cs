﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prometheus_Adventure_RPG.DataModel
{
    sealed class Info : IEvent
    {
        public string DisplayText { get; set; }

        public string NextEventKey { get; set; }
    }
}
