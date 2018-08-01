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
        private List<GradientControl> Gradients;
        private StepperGraphControl Chart;

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
            this.Gradients = new List<GradientControl>();
            foreach (VarVM var in this.StepperVM.Variables)
            {
                this.Variables.Add(new VariableControl(var));
                this.Gradients.Add(new GradientControl(var));
            }              

            this.Chart = new StepperGraphControl(new StepperGraphVM(this.StepperVM));

            ConfigureDisplay();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ChartCard.Height = this.MainGrid.ActualHeight;
        }

        //CONFIGURE DISPLAY
        private void ConfigureDisplay()
        {
            foreach (ObjectiveControl objective in this.Objectives)
                this.ObjectivesPanel.Children.Add(objective);


            for(int i=0; i < this.Variables.Count; i++)
            {
                VariableControl var = this.Variables[i];
                this.VariablesPanel.Children.Add(var);

                GradientControl grad = this.Gradients[i];
                this.GradientDataPanel.Children.Add(grad);
            }

            this.ChartPanel.Children.Add(Chart);

            this.SettingsExpander.Content = new SettingsControl(this.StepperVM);
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
            Button button = (Button)sender;
            string name = button.Name;

            if (name == "ButtonGradient")
            {
                StepperOptimizer calculator = new StepperOptimizer(this.StepperVM.Design);
                var GradientData = calculator.CalculateGradient();

                int obj = this.StepperVM.ObjIndex;

                for (int i = 0; i < GradientData[obj].Count; i++)
                    this.StepperVM.Variables[i].Gradient = GradientData[obj][i];
            }
            else
            {
                StepperOptimizer.Direction dir;

                if (name == "ButtonStepUp")
                    dir = StepperOptimizer.Direction.Maximize;
                else if (name == "ButtonStepDown")
                    dir = StepperOptimizer.Direction.Minimize;
                else
                    dir = StepperOptimizer.Direction.Isoperformance;

                this.StepperVM.Optimize(dir);
            }
        }
    }
}
