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
            //this.control = new BaseControl(radicalVM);
            this.RadicalVM = radicalVM;
            this.DataContext = this.RadicalVM;

            InitializeComponent();
            AddConstraints();
            AddNumbers();
            AddGeometries();
        }
        public RadicalVM RadicalVM;
        //public BaseControl control;
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

            foreach (ConstVM cvm in RadicalVM.Constraints)
            {
                this.Constraints.Children.Add(new ConstraintControl(cvm));
            }
        }

        //ADD NUMBERS
        //Adds a stack panel for numeric sliders
        private void AddNumbers()
        {
            //Collapse Sliders expander if no sliders are connected
            if (!RadicalVM.NumVars.Any())
            {
                this.SlidersExpander.Visibility = Visibility.Collapsed;
                return;
            }

            foreach (VarVM var in RadicalVM.NumVars)
            {
                this.Sliders.Children.Add(new VariableControl(var));
            }
        }

        //ADD GEOMETRIES
        //Adds a nested stack panel for geometries and their control point variables
        private void AddGeometries()
        {
            //Collapse Geometries expander if no geometries are connected
            if (!RadicalVM.GeoVars.Any())
            {
                this.GeometriesExpander.Visibility = Visibility.Collapsed;
                return;
            }

            int geoIndex = 1;
            foreach (List<VarVM> geometry in RadicalVM.GeoVars)
            {
                //Add an expander for each distinct geomtery
                Expander singleGeo = new Expander();
                singleGeo.Header = HeaderFormatting(String.Format("Geometry {0}", geoIndex)); geoIndex++;

                StackPanel singleGeoVars = new StackPanel { Name = "SingleGeometryVariables" };
                singleGeo.Content = singleGeoVars;

                this.Geometries.Children.Add(singleGeo);

                //Add all the variables for that geometry under its expander
                foreach (VarVM var in geometry)
                {
                    singleGeoVars.Children.Add(new VariableControl(var));
                }
            }
        }

        //Formatting individual geometry headers
        private TextBlock HeaderFormatting(string text)
        {
            TextBlock header = new TextBlock(new Run(text));
            header.Foreground = Brushes.Gray;
            header.FontSize = 14;
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

        //PREVIEW MOUSE DOWN
        //Disable changes during optimization
        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox box = (TextBox)sender;

            if (this.RadicalVM.ChangesEnabled)
                box.IsReadOnly = false;
            else
                box.IsReadOnly = true;
        }

        //GOT FOCUS
        //Clear box contents when it's active
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Disable changes during optimization
            if (this.RadicalVM.ChangesEnabled)
            {
                TextBox box = (TextBox)sender;
                box.Clear();
            }
        }

        //LOST FOCUS
        //If TextBox is left empty, set value to 0
        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;

            if (box.Text == "")
                box.Text = "0";
        }

        //PREVIEW KEY DOWN
        //Allow pressing enter to save textbox content
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //Exit the Text Box
                Keyboard.ClearFocus();
                TextBox_LostFocus(sender, e);

                //Update the value of the Text Box after exiting
                TextBox box = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;
                BindingExpression binding = BindingOperations.GetBindingExpression(box, prop);
                if (binding != null) { binding.UpdateSource(); }
            }
        }

        //PREVIVEW FLOAT INPUT
        //Only allow user to input parseable float text
        protected void TextBox_PreviewTextInput_Float(object sender, TextCompositionEventArgs e)
        {
            TextBox box = (TextBox)sender;

            //Accept negative sign only as first character
            if (box.Text == "" && e.Text == "-") { return; }

            e.Handled = !(IsTextAllowedFloat(box.Text + e.Text));
        }

        //IS TEXT ALLOWED FLOAT
        //Determine if float input is parseable
        protected static bool IsTextAllowedFloat(string text)
        {
            double val = 0;
            return double.TryParse(text, Styles.STYLEFLOAT, System.Globalization.CultureInfo.InvariantCulture, out val);
        }

        //PREVIVEW FLOAT INPUT
        //Only allow user to input parseable float text
        protected void TextBox_PreviewTextInput_Int(object sender, TextCompositionEventArgs e)
        {
            TextBox box = (TextBox)sender;

            //Accept negative sign only as first character
            if (box.Text == "" && e.Text == "-") { return; }

            e.Handled = !(IsTextAllowedInt(box.Text + e.Text));
        }

        //IS TEXT ALLOWED INT
        //Determines if int input is parseable
        protected static bool IsTextAllowedInt(string text)
        {
            int val = 0;
            return int.TryParse(text, Styles.STYLEINT, System.Globalization.CultureInfo.InvariantCulture, out val);
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

