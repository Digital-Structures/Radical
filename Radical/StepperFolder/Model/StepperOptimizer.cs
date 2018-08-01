using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Radical.Integration;

namespace Stepper
{
    //STEPPER OPTIMIZER
    public class StepperOptimizer
    {
        public enum Direction { Maximize, Minimize, Isoperformance }

        private Design Design;
        private int ObjIndex;
        private Direction Dir;
        private double StepSize;

        private int numVars;
        private int numObjs;
        private double FDstep;

        //Useful lists for value tracking
        private List<List<double>> ObjectiveData;
        private List<List<double>> IsoPerf;

        //CONSTRUCTOR for Gradient Calculation only
        public StepperOptimizer(Design design)
        {
            this.Design = design;

            numVars = Design.Variables.Count;
            numObjs = Design.Objectives.Count;
            FDstep = 0.01;

            ObjectiveData = new List<List<double>>();
            IsoPerf = new List<List<double>>();
        }

        //CONSTRUCTOR for complete step optimization
        public StepperOptimizer(Design design, int objIndex, Direction dir, double stepSize)
        {
            this.Design = design;
            this.ObjIndex = objIndex;
            this.Dir = dir;
            this.StepSize = stepSize;

            numVars = Design.Variables.Count;
            numObjs = Design.Objectives.Count;
            FDstep = 0.01;

            ObjectiveData = new List<List<double>>();
            IsoPerf = new List<List<double>>();
        }

        public List<List<double>> GenerateDesignMap()
        {
            var DifOne = new List<List<double>>();
            var DifTwo = new List<List<double>>();
            var DesignMapStepperOne = new List<List<double>>();
            var DesignMapStepperTwo = new List<List<double>>();
            var DesignMapStepperCombined = new List<List<double>>();

            //Create Design Map, list of points to be tested
            for (int i = 0; i < numVars; i++)
            {
                DesignMapStepperOne.Add(new List<double>());
                DesignMapStepperTwo.Add(new List<double>());

                for (int j = 0; j < numVars; j++)
                {
                    DesignMapStepperOne[i].Add(Design.Variables[j].CurrentValue);
                    DesignMapStepperTwo[i].Add(Design.Variables[j].CurrentValue);
                }

                IVariable var = Design.Variables[i];
                double difference = 0.5 * FDstep * (var.Max - var.Min);

                double left = var.CurrentValue - difference;
                double right = var.CurrentValue + difference;

                DesignMapStepperOne[i][i] = left;
                DesignMapStepperTwo[i][i] = right;
            }

            // Combine lists
            DesignMapStepperCombined.AddRange(DesignMapStepperOne);
            DesignMapStepperCombined.AddRange(DesignMapStepperTwo);

            // Add dummy at end to resent sliders
            DesignMapStepperCombined.Add(Design.Variables.Select(var => var.CurrentValue).ToList());

            return DesignMapStepperCombined;
        }

        public void Iterate(List<List<double>> DesignMap)
        {
            bool finished = false;

            //Invoke a delegate to solve threading issue
            System.Action run = delegate ()
            {
                foreach (List<double> sample in DesignMap)
                {
                    int i = 0;
                    foreach (double val in sample)
                    {
                        Design.Variables[i].UpdateValue(val);
                        i++;
                    }
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);

                    this.ObjectiveData.Add(Design.Objectives);
                }
                finished = true;
            };
            Rhino.RhinoApp.MainApplicationWindow.Invoke(run);

            //Wait for iteration thread to finish
            while (!finished)
            {
            }
        }

        public List<List<double>> CalculateGradient()
        {
            var DesignMap = GenerateDesignMap();
            Iterate(DesignMap);

            var Gradient = new List<List<double>>();

            double maxObj = double.MinValue;
            double minObj = double.MaxValue;

            // find the gradient for each objective by taking finite differences of every variable
            for (int j = 0; j < numObjs; j++)
            {
                Gradient.Add(new List<double>());

                for (int i = 0; i < numVars; i++)
                {
                    double left = ObjectiveData[i][j];
                    double right = ObjectiveData[numVars + i][j];

                    double difference = (right - left) / (FDstep);

                    if (difference > maxObj) { maxObj = difference; }
                    if (difference < minObj) { minObj = difference; }

                    Gradient[j].Add((double)difference);
                }


                //Normalize by max/min difference
                double maxAbs = double.MinValue;
                double vecLength = 0;

                if (Math.Abs(maxObj) > maxAbs) { maxAbs = Math.Abs(maxObj); }
                if (Math.Abs(minObj) > maxAbs) { maxAbs = Math.Abs(minObj); }

                for (int i = 0; i < numVars; i++)
                {
                    Gradient[j][i] = (Gradient[j][i] / maxAbs);
                    vecLength = vecLength + Gradient[j][i] * Gradient[j][i];
                }

                for (int i = 0; i < numVars; i++)
                {
                    Gradient[j][i] = (Gradient[j][i] / Math.Sqrt(vecLength));
                }
            }

            return Gradient;
        }

        public void Optimize()
        {
            var Gradient = this.CalculateGradient();

            //// FIND THE ORTHOGONAL VECTORS
            ////double[][] gradientArray = Gradient.Select(a => a.ToArray()).ToArray();
            List<List<string>> lst = new List<List<string>>();
            double[,] gradientArray = new double[Gradient.Count, Gradient[0].Count];

            for (int j = 0; j < Gradient.Count; j++)
            {
                for (int i = 0; i < Gradient[j].Count; i++)
                {
                    gradientArray[j, i] = Gradient[j][i];
                }
            }

            var matrixGrad = MathNet.Numerics.LinearAlgebra.Double.DenseMatrix.OfArray(gradientArray);
            var nullspace = matrixGrad.Kernel();

            // Convert array to List of nullspace vectors
            if (numVars > numObjs)
            {
                for (int i = 0; i < numVars - numObjs; i++)
                {
                    IsoPerf.Add(new List<double>());
                    double[] IsoPerfDir = nullspace[i].ToArray();
                    IsoPerf[i].AddRange(IsoPerfDir);
                }
            }

            // Randomly pick an isoperformance direction
            Random rnd = new Random();
            int dir = new int();
            int testrand = new int();
            testrand = rnd.Next(numVars - numObjs);

            dir = testrand;

            // Ensure that direction is "interesting"

            //for (int i = testrand; i < numVars - numObjs - 1; i++)
            //{

            //    dir = i;
            //    List<double> IsoVecAbs = IsoPerf[i].Select(x => Math.Abs(x)).ToList();
            //    IsoVecAbs.Sort();

            //    double a = IsoVecAbs[numVars - 1]; 
            //    double b = IsoVecAbs[numVars - 2];
            //    double c = a / b;

            //    if (c < 3) { break; }
            //    else { dir = dir + 1; }

            //}

            //for (int i = 0; i < numVars - numObjs; i++)
            //{
            //    for (int j = 0; j < IsoPerf[i].Count; j++)
            //    {
            //        IsoPerf.Add(new List<double>());
            //        double[] IsoPerfDir = nullspace[i].ToArray();

            //    }
            //}

            List<double> IsoPerfDirList = IsoPerf[dir];

            // step in the right direction based on the gradient vector

            //Set all sliders to their optimized values
            for (int i = 0; i < numVars; i++)
            {
                IVariable var = Design.Variables[i];
                double SteppedValue;

                switch(this.Dir)
                {
                    case Direction.Maximize:
                        SteppedValue = var.CurrentValue + Gradient[this.ObjIndex][i] * this.StepSize * (var.Max - var.Min);
                        var.CurrentValue = SteppedValue;
                        break;

                    case Direction.Minimize:
                        SteppedValue = var.CurrentValue - Gradient[this.ObjIndex][i] * this.StepSize * (var.Max - var.Min);
                        var.CurrentValue = SteppedValue;
                        break;

                    case Direction.Isoperformance:
                        SteppedValue = var.CurrentValue + IsoPerfDirList[i] * this.StepSize * numVars;
                        var.CurrentValue = SteppedValue;
                        break;
                }
            }
        }
    }
}
