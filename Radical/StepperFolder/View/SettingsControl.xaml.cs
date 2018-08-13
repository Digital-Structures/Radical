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
using Radical;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        private StepperWindow MyWindow;
        private StepperVM Stepper;

        public SettingsControl()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public SettingsControl(StepperVM stepper, StepperWindow window)
        {
            this.MyWindow = window;
            this.Stepper = stepper;

            this.DataContext = Stepper;
            InitializeComponent();
        }

        //SELECTION CHANGED
        //Notify the VM that the current objective changed
        private void ChosenObjective_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Stepper.FirePropertyChanged("CurrentObjectiveName");
        }

        //CHECKED
        //Enables absolute objective value graph
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            this.MyWindow.DisplayAbsolute();
        }

        //UNCHECKED
        //Enables normalized objective value graph
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.MyWindow.DisplayNormalized();
        }
    }
}
