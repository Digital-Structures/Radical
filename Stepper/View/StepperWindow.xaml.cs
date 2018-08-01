using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
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
using System.Threading;
using System.Windows.Markup;
using System.Globalization;
using System.Reflection;
using Radical;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StepperWindow : Window
    {
        //Variables and properties
        private StepperVM StepperVM;
        private List<ObjectiveControl> Objectives;
        private List<VariableControl> Variables;

        public StepperWindow() : base()
        {
            InitializeComponent();
        }

        //CONSTRUCTOR
        public StepperWindow(StepperVM svm) : base()
        {
            //Bind property data through the StepperVM
            this.StepperVM = svm;
            this.DataContext = svm;
            InitializeComponent();

            //Create a list of ObjectiveControl UI objects for each objective
            this.Objectives = new List<ObjectiveControl>();
            foreach (ObjectiveVM objective in this.StepperVM.Objectives)
                this.Objectives.Add(new ObjectiveControl(objective));

            //Create a list of VariableConrol UI objects for each variable
            this.Variables = new List<VariableControl>();
            foreach (VarVM var in this.StepperVM.Variables)
                this.Variables.Add(new VariableControl(var));

            ConfigureDisplay();
        }

        //CONFIGURE DISPLAY
        private void ConfigureDisplay()
        {
            foreach (ObjectiveControl objective in this.Objectives)
                this.ObjectivesPanel.Children.Add(objective);

            foreach (VariableControl var in this.Variables)
                this.VariablesPanel.Children.Add(var);

            this.ChartPanel.Children.Add(new StepperGraphControl(new StepperGraphVM(this.StepperVM)));
        }

        //WINDOW CLOSING
        //Alert the VM that the window has closed, another window may open
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.StepperVM.OnWindowClosing();
        }

        //BUTTON PLAY
        //Runs one step of the animation on click
        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonStep.Visibility = Visibility.Collapsed;
            this.ButtonStepping.Visibility = Visibility.Visible;

            this.StepperVM.Optimize();

            this.ButtonStep.Visibility = Visibility.Visible;
            this.ButtonStepping.Visibility = Visibility.Collapsed;
        }
    }
}
