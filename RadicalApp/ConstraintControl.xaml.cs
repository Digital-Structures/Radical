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

namespace RadicalApp
{
    /// <summary>
    /// Interaction logic for ConstraintControl.xaml
    /// </summary>
    public partial class ConstraintControl : UserControl
    {
        public ConstraintControl()
        {
            //this.DataContext = ControlVM;
            InitializeComponent();
        }

        //private ConstVM ControlVM {get; set;}

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
