using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Oracle.DataAccess.Client;

namespace Spreadsheet
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SpreadSheet s;

        public MainWindow()
        {
            InitializeComponent();
            s = new SpreadSheet(100, 100);
            workspace.Children.Add(s);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            s.Cells(0, 0).Text = "hi";
            watch.Stop();
            Console.WriteLine("Time spent: " + watch.Elapsed);
        }
    }
}
