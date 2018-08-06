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
using LiveCharts;
using LiveCharts.Wpf;
using DSOptimization;

namespace Radical
{
    public class GraphVM : BaseVM
    {
        public ChartValues<double> ChartValues { get; set; }

        public GraphVM(ChartValues<double> scores, string name)
        {
            ChartValues = scores; 
            _linegraph_name = name;
            _y = "0";
            _graphaxislabelsvisibility = Visibility.Hidden;
        }

        private String _linegraph_name;
        public String LineGraphName
        {
            get
            { return _linegraph_name;}
            set
            {
                if (CheckPropertyChanged<String>("LineGraphName", ref _linegraph_name, ref value))
                {
                }
            }
        }

        //Tells the vertical line on the chart where it should be relative to the mouse
        private int _mouseiteration;
        public int MouseIteration
        {
            get { return _mouseiteration; }
            set
            {
                if (CheckPropertyChanged<int>("MouseIteration", ref _mouseiteration, ref value))
                {
                }
            }
        }

        private string _y;
        public string DisplayY
        {
            get { return String.Format("{0}: {1}", this.LineGraphName, _y); }
            set
            {
                CheckPropertyChanged<string>("DisplayY", ref _y, ref value);
            }
        }

        public RadicalWindow _window;
        public RadicalWindow Window
        {
            get { return _window; }
            set
            {
                _window = value;
            }
        }

        public double _graphgridheight;
        public double GraphGridHeight
        {
            get { return _graphgridheight; }
            set
            {
                _graphgridheight = value;
            }
        }

        //THIS TOOL SHOULD BE IMPROVED
        private bool _optimizerdone;
        public bool OptimizerDone
        {
            get { return _optimizerdone; }
            set
            {
                _optimizerdone = value;
            }
        }

        private double _finaloptimizedvalue;
        public double FinalOptimizedValue
        {
            get { return _finaloptimizedvalue; }
            set
            {
                if(CheckPropertyChanged<double>("FinalOptimizedValue", ref _finaloptimizedvalue, ref value))
                {
                    FinalOptimizedValueString = String.Format("{0:0.000000}", FinalOptimizedValue);
                }
            }
        }

        //There has to be a better way
        private string _finaloptimizedvaluestring;
        public string FinalOptimizedValueString
        {
            get { return _finaloptimizedvaluestring; }
            set
            {
                if (CheckPropertyChanged<string>("FinalOptimizedValueString", ref _finaloptimizedvaluestring, ref value))
                {
                }
            }
        }

        public void UpdateHeight()
        {
            GraphGridHeight = Window.MainGrid.ActualHeight * 0.45;
        }

        #region Visibility

        //GRAPH VISIBILITY
        //Disables graph visibility when you don't want to see it (checkbox option)
        private Visibility _graphVisibility;
        public Visibility GraphVisibility
        {
            get
            {
                return _graphVisibility;
            }
            set
            {
                CheckPropertyChanged<Visibility>("GraphVisibility", ref _graphVisibility, ref value);
            }
        }

        private Visibility _chartlinevisiblity;
        public Visibility ChartLineVisibility
        {
            get { return _chartlinevisiblity; }
            set
            {
                if (CheckPropertyChanged<Visibility>("ChartLineVisibility", ref _chartlinevisiblity, ref value))
                {
                }
            }
        }

        private Visibility _graphaxislabelsvisibility;
        public Visibility GraphAxisLabelsVisibility
        {
            get { return _graphaxislabelsvisibility; }
            set
            {
                if (CheckPropertyChanged<Visibility>("GraphAxisLabelsVisibility", ref _graphaxislabelsvisibility, ref value))
                {
                }
            }
        }
        #endregion

    }
}