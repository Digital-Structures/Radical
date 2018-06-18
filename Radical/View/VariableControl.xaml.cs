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

namespace Radical
{
    /// <summary>
    /// Interaction logic for DesignVariableControl.xaml
    /// </summary>
    /// 

    // LOOK HERE FOR INPUT FORMAT CHANGES
    public static class Styles
    {
        public const System.Globalization.NumberStyles STYLEFLOAT = System.Globalization.NumberStyles.AllowDecimalPoint 
            | System.Globalization.NumberStyles.AllowLeadingSign;
        public const System.Globalization.NumberStyles STYLEINT = System.Globalization.NumberStyles.Integer;
    }

    public partial class VariableControl : UserControl
    {
        public VariableControl()
        {
            this.DataContext = ControlVM;
            InitializeComponent();
        }

        public VariableControl(VarVM varvm)
        {
            this.ControlVM = varvm;
            this.DataContext = ControlVM;
            InitializeComponent();
        }

        public VarVM ControlVM { get; set; }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            double val = 0;
            return double.TryParse(text, Styles.STYLEFLOAT, System.Globalization.CultureInfo.InvariantCulture, out val);
        }
    }
}
