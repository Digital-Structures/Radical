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
using System.Text.RegularExpressions;

namespace Radical
{
    /// <summary>
    /// Interaction logic for ConstraintControl.xaml
    /// </summary>
    public partial class ConstraintControl : UserControl
    {
        public ConstraintControl()
        {
            this.DataContext = ControlVM;
            InitializeComponent();
        }

        public ConstraintControl(ConstVM const_vm)
        {
            this.ControlVM = const_vm;
            this.DataContext = ControlVM;
            InitializeComponent();
        }

        private ConstVM ControlVM {get; set;}

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            double val = 0;
            return double.TryParse(text, Styles.STYLEFLOAT, System.Globalization.CultureInfo.CurrentCulture, out val);
        }
    }
}
