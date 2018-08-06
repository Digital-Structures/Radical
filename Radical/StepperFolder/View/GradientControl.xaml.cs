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
using Radical;
using DSOptimization;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for GradientControl.xaml
    /// </summary>
    public partial class GradientControl : UserControl
    {
        public GradientControl()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public GradientControl(VarVM VM)
        {
            this.DataContext = VM;
            InitializeComponent();
        }
    }
}