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

namespace Radical
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl
    {
        public GraphControl()
        {
            InitializeComponent();
        }

        //UPDATE CONSTRAINTS WINDOW 
        public void UpdateWindowGeneral(IEnumerable<double> y)
        {
            var x = Enumerable.Range(0, y.Count()).ToArray();

            Dispatcher.Invoke(() =>
            {
                Plotter.Plot(x, y);
            });
        }
    }
}
