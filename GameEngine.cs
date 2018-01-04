using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prometheus_Adventure_RPG.DataModel;
using System.Windows.Media;
using System.Windows;

namespace Prometheus_Adventure_RPG
{
    sealed class GameEngine
    {
        Config config;

        public GameEngine(Config config)
        {
            this.config = config;
        }

        public void Start()
        {
            var current = config.Events[config.StartEventKey];
            while (true)
            {
                MainWindow.WriteLine(current.DisplayText, Colors.Yellow);
                if (current is Info)
                {
                    MainWindow.Write("Press any key to continue...", Colors.Red);
                    MainWindow.WaitForKey(k => true);
                    MainWindow.WriteLine("", new Color());
                    current = config.Events[(current as Info).NextEventKey];
                }
                else if (current is Fork)
                {
                    var c = current as Fork;
                    MainWindow.WriteLine(c.Question + "\n", Colors.Red);
                    for (int i = 0; i < c.Possibilities.Count; ++i)
                        MainWindow.WriteLine($"  - [{i}]: {c.Possibilities[i].Text}", Colors.Yellow);
                    MainWindow.WriteLine("", new Color());
                    current = config.Events[c.Possibilities[MainWindow.Get("What would you like to do? ", i => i < c.Possibilities.Count && i >= 0, int.Parse)].NextKey];
                }
                else if (current is GameOver)
                {
                    MainWindow.Write("Press any key to continue...", Colors.Red);
                    MainWindow.WaitForKey(k => true);
                    Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
                }
            }
        }
    }
}
