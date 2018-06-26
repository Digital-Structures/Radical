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

    //STYLES
    //Int and Float inpute parsing styles
    public static class Styles
    {
        public const System.Globalization.NumberStyles STYLEFLOAT = System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign;
        public const System.Globalization.NumberStyles STYLEINT = System.Globalization.NumberStyles.Integer;
    }

    public partial class VariableControl : UserControl
    {
        public VariableControl()
        {
            this.DataContext = ControlVM;
            InitializeComponent();
        }

        //CONSTRUCTOR
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

        //MOUSE DOWN
        //Clear box contents when clicked
        private void TextBox_MouseDown(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;

            box.Clear();
        }

        //LOST FOCUS
        //If TextBox is left empty, set value to 0
        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;

            if(box.Text=="")
            {
                box.Text = "0";
            }
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

        //PREVIVEW TEXT INPUT
        //Only allow user to input parseable float text
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox box = (TextBox)sender;

            //Accept negative sign only as first character
            if (box.Text=="" && e.Text=="-"){return;}

            e.Handled = !(IsTextAllowed(box.Text + e.Text));
        }

        //IS TEXT ALLOWED
        //Determine if user input is parseable
        private static bool IsTextAllowed(string text)
        {
            double val = 0;
            return double.TryParse(text, Styles.STYLEFLOAT, System.Globalization.CultureInfo.InvariantCulture, out val);
        }
    }
}
