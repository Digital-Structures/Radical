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
using System.ComponentModel;
using Radical;
using Stepper;

namespace DSOptimization
{
    /// <summary>
    /// Interaction logic for DSOptimizeWindow.xaml
    /// </summary>
    public partial class DSOptimizeWindow : Window
    {
        public DSOptimizeWindow()
        {
            InitializeComponent();
        }

        DSOptimizerComponent MyComponent;

        public DSOptimizeWindow(Design design, DSOptimizerComponent component)
        {
            MyComponent = component;
            InitializeComponent();

            this.StepperTab.Content = new StepperWindow(new StepperVM(design, component));
            this.RadicalTab.Content = new RadicalWindow(new RadicalVM(design, component));
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            this.MyComponent.IsWindowOpen = false;
        }

    }
}
