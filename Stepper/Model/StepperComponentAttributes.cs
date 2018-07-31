using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Radical;
using MathNet.Numerics;

namespace Stepper
{
    class StepperComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public StepperComponent MyComponent;

        public StepperComponentAttributes(IGH_Component component)
            : base(component)
        {
            MyComponent = (StepperComponent)component;
        }

        //DOUBLE CLICK
        //Open window on double click. User should run the tool from this interface, not the grasshopper window.
        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            //Prevent opening of multiple windows at once
            if (!MyComponent.IsWindowOpen)
            {
                MyComponent.IsWindowOpen = true;
                StepperDesign design = new StepperDesign(MyComponent);
                StepperVM VM = new StepperVM(this.MyComponent, design);

                Thread viewerThread = new Thread(delegate ()
                {
                    System.Windows.Window viewer = new StepperWindow(VM);
                    viewer.Show();
                    System.Windows.Threading.Dispatcher.Run();
                });

                viewerThread.SetApartmentState(ApartmentState.STA); // needs to be STA or throws exception
                viewerThread.Start();
            }

            return base.RespondToMouseDoubleClick(sender, e);
        }
    }
}
