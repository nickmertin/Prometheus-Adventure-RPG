using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Prometheus_Adventure_RPG.DataModel;

namespace Prometheus_Adventure_RPG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static MainWindow instance;
        GameEngine game = new GameEngine(Config.Load());
        Task controller;

        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            controller = Task.Run(new Action(game.Start));
        }

        public static void Write(string s, Color c) => instance.Dispatcher.Invoke(() =>
            {
                var b = new SolidColorBrush(c);
                IEnumerable<string> strs = s.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
                if (instance.panel.Children.Count > 0)
                {
                    instance.panel.Children.OfType<TextBlock>().Last().Inlines.Add(new Run(strs.First()) { Foreground = b });
                    strs = strs.Skip(1);
                }
                foreach (var l in strs)
                    instance.panel.Children.Add(new TextBlock { Text = l, Foreground = b });
            });

        public static void Write(object o, Color c) => Write(o.ToString(), c);

        public static void WriteLine(object o, Color c) => Write(o.ToString() + "\n", c);

        public static string ReadLine(Color c)
        {
            Run t = null;
            int offset = 0;
            KeyEventHandler h = null;
            using (var ewh = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                h = (sender, e) =>
                {
                    switch (e.Key)
                    {
                        case Key.Back:
                            if (offset > 0)
                                t.Text = t.Text.Remove(--offset, 1);
                            break;
                        case Key.Delete:
                            if (offset < t.Text.Length)
                                t.Text = t.Text.Remove(offset, 1);
                            break;
                        case Key.Left:
                            if (offset > 0)
                                --offset;
                            break;
                        case Key.Right:
                            if (offset < t.Text.Length)
                                ++offset;
                            break;
                        case Key.Enter:
                            instance.KeyDown -= h;
                            instance.panel.Children.Add(new TextBlock());
                            ewh.Set();
                            break;
                        default:
                            t.Text = t.Text.Insert(offset++, KeyUtility.GetCharFromKey(e.Key).ToString());
                        break;
                    }
                };
                instance.Dispatcher.Invoke(() =>
                    {
                        t = new Run() { Foreground = new SolidColorBrush(c) };
                        if (instance.panel.Children.Count > 0)
                            instance.panel.Children.Cast<TextBlock>().Last().Inlines.Add(t);
                        else
                            instance.panel.Children.Add(new TextBlock(t));
                        instance.KeyDown += h;
                    });
                ewh.WaitOne();
                return t.Dispatcher.Invoke(() => t.Text);
            }
        }

        public static void WaitForKey(Predicate<Key> predicate)
        {
            KeyEventHandler h = null;
            using (var ewh = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                h = (sender, e) =>
                    {
                        if (predicate(e.Key))
                        {
                            instance.KeyDown -= h;
                            ewh.Set();
                        }
                    };
                instance.Dispatcher.Invoke(() => instance.KeyDown += h);
                ewh.WaitOne();
            }
        }

        public static void Clear() => instance.Dispatcher.Invoke(() => instance.panel.Children.Clear());

        public static T Get<T>(string prompt, Predicate<T> condition, Func<string, T> parser)
        {
            bool started = false;
            T result;
            do
            {
                if (started)
                    WriteLine("Invalid input!", Colors.Red);
                else
                    started = true;
                _try:
                try
                {
                    Write(prompt, Colors.Red);
                    result = parser(ReadLine(Colors.Magenta));
                }
                catch
                {
                    WriteLine("Invalid input!", Colors.Red);
                    goto _try;
                }
            } while (!condition(result));
            return result;
        }
    }
}
