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
using Radical.Integration;
using System.Threading;
using System.Windows.Markup;
using NLoptNet;
using System.Globalization;
using System.Reflection;

namespace Radical
{
    /// <summary>
    /// Interaction logic for MaiWindow.xaml
    /// </summary>
    public partial class RadicalWindow : Window
    {
        public RadicalWindow()
        {
            this.DataContext = RadicalVM;
            InitializeComponent();
        }

        //CONSTRUCTOR
        public RadicalWindow(RadicalVM radicalVM)
        {
            this.RadicalVM = radicalVM;
            this.DataContext = this.RadicalVM;

            InitializeComponent();
            AddConstraints();
            AddNumbers();
            AddGeometries();
            this.SettingsMenu.Children.Add(new SettingsControl(this.RadicalVM));
        }
        public enum Direction { X, Y, Z, None };
        public RadicalVM RadicalVM;
        public CancellationTokenSource source;

        //ADD CONSTRAINTS
        //Adds a stack panel for constraints
        private void AddConstraints()
        {
            //Collapse Constraints expander if no constraints are imposed
            if (!RadicalVM.Constraints.Any())
            {
                this.ConstraintsExpander.Visibility = Visibility.Collapsed;
                return;
            }

            this.Constraints.Children.Add(new VariableHeaderControl());

            foreach (ConstVM cvm in RadicalVM.Constraints)
            {
                this.Constraints.Children.Add(new ConstraintControl(cvm));
            }
        }

        //ADD NUMBERS
        //Adds a stack panel for numeric sliders
        private void AddNumbers()
        {
            //COLLAPSE SLIDERS
            //Collapse Sliders expander if no sliders are connected
            if (!RadicalVM.NumVars.Any())
            {
                this.SlidersExpander.Visibility = Visibility.Collapsed;
                return;
            }

            //GROUP VARIABLE CONTROL
            //Stack Panel
            StackPanel groupControls = new StackPanel();
            //Expander
            Expander groupControlMenu = new Expander();
            groupControlMenu.Header = Header2Formatting("Group Variable Control");
            groupControlMenu.Content = groupControls;
            this.Sliders.Children.Add(groupControlMenu);

            //Add descriptive control labels
            groupControls.Children.Add(new VariableHeaderControl());

            //Add group controls for X, Y, and Z directions
            GroupVariableControl groupControl = new GroupVariableControl(new GroupVarVM(this.RadicalVM, (int)(Direction.None)));
            groupControl.GroupControlName.Text = "All Variables";
            groupControls.Children.Add(groupControl);

            //INDIVIDUAL VARIBALE CONTROL
            //Stack Panel
            StackPanel individualControls = new StackPanel();
            //Expander
            Expander individualControlMenu = new Expander();
            individualControlMenu.Header = Header2Formatting("Single Variable Control");
            individualControlMenu.Content = individualControls;
            this.Sliders.Children.Add(individualControlMenu);

            //Add descriptive control labels
            individualControls.Children.Add(new VariableHeaderControl());

            //Add individual slider controls
            foreach (VarVM var in RadicalVM.NumVars)
                individualControls.Children.Add(new VariableControl(var));
        }

        //ADD GEOMETRIES
        //Adds a nested stack panel for geometries and their control point variables
        private void AddGeometries()
        {
            //COLLAPSE GEOMETRIES
            //Collapse Geometries expander if no geometries are connected
            if (!RadicalVM.GeoVars.Any())
            {
                this.GeometriesExpander.Visibility = Visibility.Collapsed;
                return;
            }

            int geoIndex = 0;
            foreach (List<VarVM> geometry in RadicalVM.GeoVars)
            {
                geoIndex++;

                //SINGLE GEOMETRY
                //Stack Panel
                StackPanel variableMenus = new StackPanel();
                //Expander
                Expander singleGeo = new Expander();
                singleGeo.Header = Header1Formatting(String.Format("Geometry {0}", geoIndex));
                singleGeo.Content = variableMenus;
                this.Geometries.Children.Add(singleGeo);


                //GROUP VARIABLE CONTROL
                //Stack Panel
                StackPanel groupControls = new StackPanel();
                //Expander
                Expander groupControlMenu = new Expander();
                groupControlMenu.Header = Header2Formatting("Group Variable Control");
                groupControlMenu.Content = groupControls;
                variableMenus.Children.Add(groupControlMenu);

                //Add descriptive control labels
                groupControls.Children.Add(new VariableHeaderControl());

                //Add group controls for X, Y, and Z directions
                GroupVariableControl groupControlX = new GroupVariableControl(new GroupVarVM(this.RadicalVM, (int)(Direction.X), geoIndex-1));
                GroupVariableControl groupControlY = new GroupVariableControl(new GroupVarVM(this.RadicalVM, (int)(Direction.Y), geoIndex-1));
                GroupVariableControl groupControlZ = new GroupVariableControl(new GroupVarVM(this.RadicalVM, (int)(Direction.Z), geoIndex-1));
                groupControlX.GroupControlName.Text = "X Variables";
                groupControlY.GroupControlName.Text = "Y Variables";
                groupControlZ.GroupControlName.Text = "Z Variables";
                groupControls.Children.Add(groupControlX);
                groupControls.Children.Add(groupControlY);
                groupControls.Children.Add(groupControlZ);

                //INDIVIDUAL VARIBALE CONTROL
                //Stack Panel
                StackPanel individualControls = new StackPanel();
                //Expander
                Expander individualControlMenu = new Expander();
                individualControlMenu.Header = Header2Formatting("Single Variable Control");
                individualControlMenu.Content = individualControls;
                variableMenus.Children.Add(individualControlMenu);

                //Add descriptive control labels
                individualControls.Children.Add(new VariableHeaderControl());

                //Add individual point controls in all directions
                foreach (VarVM var in geometry)
                    individualControls.Children.Add(new VariableControl(var));
            }
        }

        //Formatting individual geometry headers
        private TextBlock Header1Formatting(string text)
        {
            TextBlock header = new TextBlock(new Run(text));
            header.Foreground = Brushes.Gray;
            header.FontSize = 16;
            header.Margin = new Thickness(1, 0, 0, 0);

            return header;
        }

        //Formatting control group headers
        private TextBlock Header2Formatting(string text)
        {
            TextBlock header = new TextBlock(new Run(text));
            header.Foreground = Brushes.DarkGray;
            header.FontSize = 16;
            header.Margin = new Thickness(1, 0, 0, 0);

            return header;
        }

        //OPTIMIZATION STARTED
        public void OptimizationStarted()
        {
            this.RadicalVM.OptimizationStarted();
        }

        //OPTIMIZATION FINISHED
        public void OptimizationFinished()
        {
            this.RadicalVM.OptimizationFinished();
        }

        //OPTIMIZE
        void Optimize()
        {
            this.RadicalVM.Design.Optimize(this);
        }

        //UPDATE WINDOW
        public void UpdateWindow(IEnumerable<double> y)
        {
            var x = Enumerable.Range(0, y.Count()).ToArray();

            Dispatcher.Invoke(() =>
            {
                Plotter.Plot(x, y);
            });
        }

        //WINDOW CLOSING
        //Alert the component that the window has been closed
        //(and therefore a new window can open on double click)
        public void RadicalWindow_Closing(object sender, CancelEventArgs e)
        {
            this.RadicalVM.OnWindowClosing();
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

        //BUTTON PLAY CLICK
        private async void ButtonPlay_Click(object sender, RoutedEventArgs e) // make async
        {
            if (this.RadicalVM.Design.ActiveVariables.Any())
            {
                ButtonPause.Visibility = Visibility.Visible;
                ButtonPlay.Visibility = Visibility.Collapsed;
                source = new CancellationTokenSource();

                try
                {
                    await Task.Run(() => Optimize(), source.Token);

                }
                catch (OperationCanceledException)
                {

                }
                UpdateWindow(this.RadicalVM.Design.ScoreEvolution);
                ButtonPause.Visibility = Visibility.Collapsed;
                ButtonPlay.Visibility = Visibility.Visible;
            }
            else
                System.Windows.MessageBox.Show("No variables selected!");
        }

        //BUTTON PAUSE CLICK
        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            this.OptimizationFinished(); //Enable variable changes when paused
            source.Cancel();
            UpdateWindow(this.RadicalVM.Design.ScoreEvolution);
            ButtonPause.Visibility = Visibility.Collapsed;
            ButtonPlay.Visibility = Visibility.Visible;
        }

        //BUTTON SETTINGS OPEN CLICK
        private void ButtonSettingsOpen_Click(object sender, RoutedEventArgs e)
        {
            ButtonSettingsOpen.Visibility = Visibility.Collapsed;
            SettingsClose.Visibility = Visibility.Visible;

        }

        //BUTTON SETTINGS CLOSE CLICK
        private void ButtonSettingsClose_Click(object sender, RoutedEventArgs e)
        {
            ButtonSettingsOpen.Visibility = Visibility.Visible;
            SettingsClose.Visibility = Visibility.Collapsed;
        }

        private void OpenOptSettings(object sender, RoutedEventArgs e)
        {

        }

        private void Export_SVG(object sender, RoutedEventArgs e)
        {

        }

        private void Export_CSV(object sender, RoutedEventArgs e)
        {

        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum RefreshMode
    {
        [Description("Live Geometry and Data")] Live = 1,
        [Description("Live Data")] Data = 2,
        [Description("Silent")] Silent = 3
    }

    #region Converters
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return ((attributes.Length > 0) && (!String.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                    }
                }
                return string.Empty;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;
        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType)
                {
                    if (null != value)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                        if (!enumType.IsEnum)
                            throw new ArgumentException("Type must be for an Enum.");
                    }
                    this._enumType = value;
                }
            }
        }
        public EnumBindingSourceExtension() { }
        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this._enumType)
                throw new InvalidOperationException("The EnumType must be specified.");
            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            Array enumValues = Enum.GetValues(actualEnumType);
            if (actualEnumType == this._enumType)
                return enumValues;
            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }

    public class VisibilityToCheckedConverter : IValueConverter

    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {

            return ((Visibility)value) == Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {

            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;

        }

    }
    #endregion
}