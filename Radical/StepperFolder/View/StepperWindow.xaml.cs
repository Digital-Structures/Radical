﻿using System;
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
using DSOptimization;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StepperWindow : UserControl
    {
        //Variables and properties
        private StepperVM StepperVM;
        private List<ObjectiveControl> Objectives;
        private List<VariableControl> Variables;
        private List<GradientControl> Gradients;
        private List<GroupVariableControl> GroupVars;

        private StepperGraphControl Chart;
        private List<SolidColorBrush> ChartColors;

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
            this.GroupVars = new List<GroupVariableControl>();
            this.Variables = new List<VariableControl>();
            this.Gradients = new List<GradientControl>();
            foreach (VarVM var in this.StepperVM.Variables)
            {
                this.Variables.Add(new VariableControl(var));
                this.Gradients.Add(new GradientControl(var));
            }

            GenerateChartColors();
            this.StepperVM.ObjectiveChart = new StepperGraphVM(this.StepperVM, this.ChartColors);
            this.Chart = new StepperGraphControl(this.StepperVM.ObjectiveChart);

            ConfigureDisplay();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ChartCard.Height = this.MainGrid.ActualHeight;
        }

        //GENERATE CHART COLORS
        //Establish list of theme colors to use for objective graphs
        private void GenerateChartColors()
        {
            this.ChartColors = new List<SolidColorBrush>();

            SolidColorBrush Stroke1 = (SolidColorBrush)this.FindResource("SecondaryAccentBrush");
            this.ChartColors.Add(Stroke1);

            SolidColorBrush Stroke2 = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
            this.ChartColors.Add(Stroke2);

            SolidColorBrush Stroke3 = (SolidColorBrush)this.FindResource("PrimaryHueMidBrush");
            this.ChartColors.Add(Stroke3);

            SolidColorBrush Stroke4 = (SolidColorBrush)this.FindResource("PrimaryHueLightBrush");
            this.ChartColors.Add(Stroke4);
        }

        //CONFIGURE DISPLAY
        private void ConfigureDisplay()
        {
            AddNumbers();
            AddGeometries();

            //Add created GroupVarVMs to StepperVM
            foreach (GroupVariableControl group in this.GroupVars)
                this.StepperVM.GroupVars.Add((GroupVarVM)group.MyVM);

            //Add objective controls
            foreach (ObjectiveControl objective in this.Objectives)
                this.ObjectivesPanel.Children.Add(objective);

            //Add gradient info controls
            for(int i=0; i < this.Variables.Count; i++)
            {
                GradientControl grad = this.Gradients[i];
                this.GradientDataPanel.Children.Add(grad);
            }

            this.ChartPanel.Children.Add(Chart);

            this.SettingsExpander.Content = new SettingsControl(this.StepperVM);
        }

        //ADD NUMBERS
        //Adds a stack panel for numeric sliders
        private void AddNumbers()
        {
            //COLLAPSE SLIDERS
            //Collapse Sliders expander if no sliders are connected
            if (!StepperVM.NumVars.Any())
            {
                this.SlidersExpander.Visibility = Visibility.Collapsed;
                return;
            }

            //GROUP VARIABLE CONTROL
            //Stack Panel
            StackPanel groupControls = new StackPanel();
            groupControls.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
            //Expander
            Expander groupControlMenu = new Expander();
            groupControlMenu.Background = (SolidColorBrush)this.FindResource("BackgroundHueDarkBrush");
            groupControlMenu.IsExpanded = true;
            groupControlMenu.Header = Header2Formatting("Group Variable Control");
            groupControlMenu.Content = groupControls;
            this.Sliders.Children.Add(groupControlMenu);

            //Add descriptive control labels
            groupControls.Children.Add(new VariableHeaderControl());

            //Add group controls for X, Y, and Z directions
            GroupVariableControl groupControl = new GroupVariableControl(new GroupVarVM(this.StepperVM, (int)(VarVM.Direction.None))); this.GroupVars.Add(groupControl);
            groupControl.GroupControlName.Text = "All Variables";
            groupControls.Children.Add(groupControl);

            //Border separator
            this.Sliders.Children.Add(this.Separator());

            //INDIVIDUAL VARIBALE CONTROL
            //Stack Panel
            StackPanel individualControls = new StackPanel();
            individualControls.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
            //Expander
            Expander individualControlMenu = new Expander();
            individualControlMenu.Background = (SolidColorBrush)this.FindResource("BackgroundHueDarkBrush");
            individualControlMenu.Header = Header2Formatting("Single Variable Control");
            individualControlMenu.Content = individualControls;
            this.Sliders.Children.Add(individualControlMenu);

            //Add descriptive control labels
            individualControls.Children.Add(new VariableHeaderControl());

            //Add individual slider controls
            foreach (VarVM var in StepperVM.NumVars)
                individualControls.Children.Add(new VariableControl(var));
        }

        //ADD GEOMETRIES
        //Adds a nested stack panel for geometries and their control point variables
        private void AddGeometries()
        {
            //COLLAPSE GEOMETRIES
            //Collapse Geometries expander if no geometries are connected
            if (!StepperVM.GeoVars.Any())
            {
                this.GeometriesExpander.Visibility = Visibility.Collapsed;
                return;
            }

            int geoIndex = 0;

            foreach (List<VarVM> geometry in StepperVM.GeoVars)
            {

                //SINGLE GEOMETRY
                //Stack Panel
                StackPanel variableMenus = new StackPanel();
                variableMenus.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
                //Expander
                Expander singleGeo = new Expander();

                singleGeo.Background = (SolidColorBrush)this.FindResource("BackgroundHueMidBrush");
                singleGeo.Header = Header1Formatting(geometry[geoIndex].Name.Split('.')[0]); geoIndex++;
                singleGeo.Content = variableMenus;
                this.Geometries.Children.Add(singleGeo);

                //Border
                variableMenus.Children.Add(this.Separator());


                //GROUP VARIABLE CONTROL
                //Stack Panel
                StackPanel groupControls = new StackPanel();
                groupControls.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
                //Expander
                Expander groupControlMenu = new Expander();
                groupControlMenu.IsExpanded = true;
                groupControlMenu.Header = Header2Formatting("Group Variable Control");
                groupControlMenu.Background = (SolidColorBrush)this.FindResource("BackgroundHueDarkBrush");
                groupControlMenu.Content = groupControls;
                variableMenus.Children.Add(groupControlMenu);

                //Add descriptive control labels
                groupControls.Children.Add(new VariableHeaderControl());

                //Add group controls for X, Y, and Z directions
                GroupVariableControl groupControlX = new GroupVariableControl(new GroupVarVM(this.StepperVM, (int)(VarVM.Direction.X), geoIndex - 1)); this.GroupVars.Add(groupControlX);
                GroupVariableControl groupControlY = new GroupVariableControl(new GroupVarVM(this.StepperVM, (int)(VarVM.Direction.Y), geoIndex - 1)); this.GroupVars.Add(groupControlY);
                GroupVariableControl groupControlZ = new GroupVariableControl(new GroupVarVM(this.StepperVM, (int)(VarVM.Direction.Z), geoIndex - 1)); this.GroupVars.Add(groupControlZ);

                groupControlX.GroupControlName.Text = "X Variables";
                groupControlY.GroupControlName.Text = "Y Variables";
                groupControlZ.GroupControlName.Text = "Z Variables";
                groupControls.Children.Add(groupControlX);
                groupControls.Children.Add(groupControlY);
                groupControls.Children.Add(groupControlZ);

                //Border separator
                variableMenus.Children.Add(this.Separator());

                //INDIVIDUAL VARIBALE CONTROL
                //Stack Panel
                StackPanel individualControls = new StackPanel();
                individualControls.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
                //Expander
                Expander individualControlMenu = new Expander();
                individualControlMenu.Header = Header2Formatting("Single Variable Control");
                individualControlMenu.Content = individualControls;
                individualControlMenu.Background = (SolidColorBrush)this.FindResource("BackgroundHueDarkBrush");
                variableMenus.Children.Add(individualControlMenu);

                //Add descriptive control labels
                individualControls.Children.Add(new VariableHeaderControl());

                //Add individual point controls in all directions
                foreach (VarVM var in geometry)
                {
                    VariableControl vc = new VariableControl(var);
                    vc.Background = (SolidColorBrush)this.FindResource("PrimaryHueDarkBrush");
                    individualControls.Children.Add(vc);
                }
            }
        }

        //Formatting methods for wpf appearance
        #region Header Formatting
        //Formatting individual geometry headers
        private TextBlock Header1Formatting(string text)
        {
            TextBlock header = new TextBlock(new Run(text));
            header.Foreground = (SolidColorBrush)this.FindResource("LightTextBrush");
            header.FontSize = 16;

            return header;
        }

        //Formatting control group headers
        private TextBlock Header2Formatting(string text)
        {
            TextBlock header = new TextBlock(new Run(text));
            header.Foreground = (SolidColorBrush)this.FindResource("PrimaryHueDarkForegroundBrush");
            header.FontSize = 16;

            return header;
        }

        //Formatting border separators
        private Border Separator()
        {
            Border b = new Border();
            b.Height = 1;
            b.HorizontalAlignment = HorizontalAlignment.Stretch;
            b.SnapsToDevicePixels = true;
            b.BorderThickness = new Thickness(0, 0, 0, 2);
            b.BorderBrush = (SolidColorBrush)this.FindResource("BackgroundHueMidBrush");
            return b;
        }
        #endregion

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
            if (this.GraphSlider.Visibility == Visibility.Hidden)
                this.GraphSlider.Visibility = Visibility.Visible;

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

        //BUTTON RESET
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            this.StepperVM.Reset();
        }

        //CLOSE MENU CLICK
        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        //OPEN MENU CLICK
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        //KEY DOWN
        //For stepping up and down with keyboard errors instead of buttons
        private void MainGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                this.ButtonPlay_Click(this.ButtonStepUp, new RoutedEventArgs());
            else if (e.Key == Key.Down)
                this.ButtonPlay_Click(this.ButtonStepDown, new RoutedEventArgs());
            else if (e.Key == Key.Left || e.Key == Key.Right)
                this.ButtonPlay_Click(this.ButtonStepIso, new RoutedEventArgs());
        }
    }
}