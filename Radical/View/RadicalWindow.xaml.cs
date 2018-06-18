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

        // USELESS CONSTRUCTOR
        public RadicalWindow()
        {
            this.DataContext = RadicalVM;
            InitializeComponent();
        }

        public RadicalWindow(RadicalVM radicalVM)
        {
            this.RadicalVM = radicalVM;
            this.DataContext = this.RadicalVM;

            InitializeComponent();
            AddConstraints();
            AddVariables();
        }
        public RadicalVM RadicalVM;

        private void AddConstraints()
        {
            foreach (ConstVM cvm in RadicalVM.Constraints)
            {
                this.Constraints.Children.Add(new ConstraintControl(cvm));
            }
        }

        private void AddVariables()
        {
            foreach (VarVM vvm in RadicalVM.Variables)
            {
                IVariable variable = vvm.DesignVar;
                if (variable is SliderVariable)
                {
                    this.Sliders.Children.Add(new VariableControl(vvm));
                    if (this.SlidersExpander.Visibility == Visibility.Collapsed)
                    {
                        this.SlidersExpander.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    this.Geometries.Children.Add(new VariableControl(vvm));
                    if (this.GeometriesExpander.Visibility == Visibility.Collapsed)
                    {
                        this.GeometriesExpander.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
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


        private async void ButtonPlay_Click(object sender, RoutedEventArgs e) // make async
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

        public CancellationTokenSource source;

        void Optimize()
        {
            this.RadicalVM.Design.Optimize(this);
        }

        public void UpdateWindow(IEnumerable<double> y)
        {
            var x = Enumerable.Range(0, y.Count()).ToArray();

            Dispatcher.Invoke(() =>
            {
                Plotter.Plot(x, y);
            });

        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            source.Cancel();
            UpdateWindow(this.RadicalVM.Design.ScoreEvolution);
            ButtonPause.Visibility = Visibility.Collapsed;
            ButtonPlay.Visibility = Visibility.Visible;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

        private void ButtonSettingsOpen_Click(object sender, RoutedEventArgs e)
        {
            ButtonSettingsOpen.Visibility = Visibility.Collapsed;
            SettingsClose.Visibility = Visibility.Visible;

        }

        private void ButtonSettingsClose_Click(object sender, RoutedEventArgs e)
        {
            ButtonSettingsOpen.Visibility = Visibility.Visible;
            SettingsClose.Visibility = Visibility.Collapsed;
        }

        private void TextBox_PreviewTextInput_Float(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedFloat(e.Text);
        }

        private static bool IsTextAllowedFloat(string text)
        {
            double val = 0;
            return double.TryParse(text, Styles.STYLEFLOAT, System.Globalization.CultureInfo.CurrentCulture, out val);
        }

        private void TextBox_PreviewTextInput_Int(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedInt(e.Text);
        }

        private static bool IsTextAllowedInt(string text)
        {
            double val = 0;
            return double.TryParse(text, Styles.STYLEINT, System.Globalization.CultureInfo.CurrentCulture, out val);
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

