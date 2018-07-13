﻿using System;
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
            this.DataContext = GraphVM;
            InitializeComponent();
        }

        public GraphControl(GraphVM graphVM, RadicalVM radicalVM)
        {
            this.RadicalVM = radicalVM;
            this.GraphVM = graphVM;
            this.DataContext = graphVM;
            InitializeComponent();

            this.GraphVM.Plotter = Plotter;
            this.GraphVM.ChartLine = ChartLine;
            this.GraphVM.ChartLineVisibility = Visibility.Collapsed;
        }

        GraphVM GraphVM;
        RadicalVM RadicalVM;

        //UPDATE CONSTRAINTS WINDOW 
        public void UpdateWindowGeneral(IEnumerable<double> y)
        {
            var x = Enumerable.Range(0, y.Count()).ToArray();

            Dispatcher.Invoke(() =>
            {
                Plotter.Plot(x, y);
            });
        }

        //CHART MOUSE DOWN
        //Currently returns graph x value of where the mouse clicks down 
        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.GraphVM.GraphScores.Any())
            {
                double mouseX = e.GetPosition(this.Plotter).X + Plotter.OffsetX;
                double ScaleX = Plotter.ScaleX;

                int actualX = (int)(Math.Truncate(mouseX / ScaleX));
                if (actualX < 0)
                {
                    actualX = 0;
                }
                else if (actualX >= this.GraphVM.GraphScores.Count)
                {
                    actualX = this.GraphVM.GraphScores.Count - 1;
                }

                this.GraphVM.UpdateLine(actualX);
                this.RadicalVM.UpdateGraphLines(actualX);
            }
            else
            {
                this.GraphVM.ChartLineVisibility = Visibility.Collapsed;
            }
        }
    }
}
