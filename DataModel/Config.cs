using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Prometheus_Adventure_RPG.DataModel
{
    sealed class Config
    {
        public static Config Load() => Application.LoadComponent(new Uri("/DataModel/Config.xaml", UriKind.Relative)) as Config;

        public string StartEventKey { get; set; }

        public Dictionary<string, IEvent> Events { get; set; } = new Dictionary<string, IEvent>();
    }
}