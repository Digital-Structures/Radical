using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI;

namespace Radical.Integration
{
    public class SliderVariable : IVariable
    {
        public SliderVariable()
        { }

        public SliderVariable(IGH_Param param)
        {
            this.Parameter = param;
            this.Slider = this.Parameter as GH_NumberSlider;
        }

        public double CurrentValue
        {
            get {return (double)Slider.CurrentValue; }
            set { }
        }

        public IGH_Param Parameter
        {
            get;
            set;
        }

        public double Max
        {
            get
            {
                return (double)Slider.Slider.Maximum;
            }
            set { }
        }

        public double Min
        {
            get
            {
                return (double)Slider.Slider.Minimum;
            }
            set { }
        }


        public GH_NumberSlider Slider;

        public double Gradient()
        {
            throw new NotImplementedException();
        }

        public void UpdateValue(double x)
        {
            try
            {
                Slider.SetSliderValue((decimal)x);
            }
            catch
            {
            }

        }

        public void UpdateMin(double x)
        {
            this.Slider.Slider.Minimum = (decimal)x;
        }

        public void UpdateMax(double x)
        {
            this.Slider.Slider.Maximum = (decimal)x;
        }
    }
}
