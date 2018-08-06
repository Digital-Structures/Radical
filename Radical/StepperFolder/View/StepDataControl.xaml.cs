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
using DSOptimization;

namespace Stepper
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StepDataControl : UserControl
    {
        private IStepDataElement MyData;

        public StepDataControl()
        {
            InitializeComponent();
        }

        public StepDataControl(IStepDataElement data)
        {
            this.MyData = data;

            MyData.PropertyChanged += new PropertyChangedEventHandler(VarPropertyChanged);
            
            InitializeComponent();

            this.Value = MyData.Value;
            this.VariableName = MyData.Name;
        }

        //Property Changed event handling method
        private void VarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.VariableName = this.MyData.Name;
        }

        //VALUE
        private double val;
        public double Value
        {
            get { return val; }
            set
            {
                if (value != val)
                {
                    val = value;
                    this.ValueText.Text = String.Format("{0:0.00}", val);
                }

            }
        }

        //NAME
        private string name;
        public string VariableName
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    this.VarName.Text = name;
                }

            }
        }
    }
}
