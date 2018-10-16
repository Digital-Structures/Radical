using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLoptNet;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using DSOptimization;

namespace Radical
{
    //RADICAL OPTIMIZER 
    public class RadicalOptimizer

    {
        public Design Design;
        public uint nVars { get { return (uint)this.Design.ActiveVariables.Count; } }

        public NLoptAlgorithm MainAlg;
        public NLoptAlgorithm SecondaryAlg; // Optional
        public NLoptSolver Solver;
        public RadicalVM RadicalVM;
        public RadicalWindow RadicalWindow; 

        public ChartValues<double> StoredMainValues;
        public ChartValues<ChartValues<double>> StoredConstraintValues;

        //static bool verbose = true;
        public double? MinValue; //init objective

        // CONSTRUCTOR FOR RADICAL
        public RadicalOptimizer(Design design, RadicalWindow radwindow)
        {
            this.Design = design;
            this.RadicalWindow = radwindow;
            this.RadicalVM = this.RadicalWindow.RadicalVM;
            this.MainAlg = this.RadicalVM.PrimaryAlgorithm;
            //this.SecondaryAlg = NLoptAlgorithm.LN_COBYLA;
            BuildWrapper();
            SetBounds();

            StoredMainValues = new ChartValues<double>();
            StoredConstraintValues = new ChartValues<ChartValues<double>>();

            if (Design.Constraints != null)
            {
                foreach (Constraint c in Design.Constraints)
                {
                    if (c.IsActive)
                    {
                        StoredConstraintValues.Add(new ChartValues<double>());
                        if (c.MyType == Constraint.ConstraintType.lessthan)
                            Solver.AddLessOrEqualZeroConstraint((x) => constraint(x, c));
                        else if (c.MyType == Constraint.ConstraintType.morethan)
                            Solver.AddLessOrEqualZeroConstraint((x) => -constraint(x, c));
                        else
                            Solver.AddEqualZeroConstraint((x) => constraint(x, c));
                    }
                }
            }

            Solver.SetMinObjective((x) => Objective(x));
        }

        public void BuildWrapper()
        {
            this.Solver = new NLoptSolver(MainAlg, nVars, this.RadicalVM.ConvCrit, this.RadicalVM.Niterations);
        }
        public void BuildWrapper(double relStopTol, int niter)
        {
            this.Solver = new NLoptSolver(MainAlg, nVars, relStopTol, niter);
        } // relStopTO, neither should be made optional arguments

        public void SetBounds()
        {
            Solver.SetLowerBounds(Design.Variables.Select(x => x.Min).ToArray());
            Solver.SetUpperBounds(Design.Variables.Select(x => x.Max).ToArray());
        }

        public double Objective(double[] x)
        {
            bool finished = false;

            Grasshopper.Kernel.GH_SolutionMode refresh = Grasshopper.Kernel.GH_SolutionMode.Silent;

            if (this.RadicalVM.Mode == RefreshMode.Live) { refresh = Grasshopper.Kernel.GH_SolutionMode.Default; }

            System.Action run = delegate ()
            {
                //Grasshopper.Instances.ActiveCanvas.Enabled = false;

                for (int i = 0; i < nVars; i++)
                {
                    IVariable var = Design.ActiveVariables[i]; // ONLY INCL. VARS
                    var.UpdateValue(x[i]);
                }
                // Once all points have been updated, we can update the geometries
                foreach (IDesignGeometry geo in this.Design.Geometries)
                {
                    geo.Update();
                }

                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true, refresh);

                finished = true;
            };
            Rhino.RhinoApp.MainApplicationWindow.Invoke(run);

            // inelegant, temporary
            while (!finished)
            {
                Thread.Sleep(1);
            }

            double objective = Design.Objectives[0];

            //When a new global objective minimum is reached all variable values at that point are recorded
            if (objective < this.RadicalVM.SmallestObjectiveValue && AllConstraintsSatisfied())
            {
                this.RadicalVM.SmallestObjectiveValue = objective; 
                foreach (VarVM v in this.RadicalVM.NumVars)
                {
                    v.UpdateBestSolutionValue();
                }
                foreach (List<VarVM> lvm in this.RadicalVM.GeoVars)
                {
                    foreach (VarVM v in lvm)
                    {
                        v.UpdateBestSolutionValue();
                    }
                }
            }

            //If RefreshMode == Silent then values are stored in a local list so that the graph is not updated
            if (this.RadicalVM.Mode == RefreshMode.Silent)
            {
                this.StoredMainValues.Add(objective);

                for (int i = 0; i < this.RadicalVM.ConstraintsEvolution.Count; i++)
                {
                    double score = Design.Constraints[i].CurrentValue;
                    this.StoredConstraintValues[i].Add(score);
                }
            }
            else
            {
                this.RadicalVM.UpdateCurrentScoreDisplay();

                //If refresh mode is switched from Silent to Live 
                if (StoredMainValues.Any())
                {
                    AppendStoredValues();
                    this.RadicalVM.AutomateStepSize(true);
                }

                //Checks to see if # of iterations is = number of original steps in graph, if it is it will make it auto
                this.RadicalVM.AutomateStepSize(false);

                //Adds main objective values to list and draws
                this.RadicalVM.ObjectiveEvolution.Add(objective);

                for (int i = 0; i < this.RadicalVM.ConstraintsEvolution.Count; i++)
                {
                    double score = Design.Constraints[i].CurrentValue;
                    this.RadicalVM.ConstraintsEvolution[i].Add(score);
                }
            }

            try
            {
                this.RadicalWindow.source.Token.ThrowIfCancellationRequested();
            }
            catch
            {
<<<<<<< HEAD
                throw new OperationCanceledException();
=======
                //throw;
                //System.Windows.MessageBox.Show("Giello!");
>>>>>>> 8b8dbf84b67b2f9590a83499fe3e0b22ccf2fe30
            }

            return objective;
        }

        public bool AllConstraintsSatisfied()
        {
            return true; 
        }

        public NloptResult RunOptimization()
        {
            //STARTED
            this.RadicalWindow.OptimizationStarted();

            // Run optimization with only the activeVariables
            double[] x = Design.ActiveVariables.Select(t => t.CurrentValue).ToArray();
            double[] query = x;
            double startingObjective = Design.Objectives[0];
            NloptResult result = Solver.Optimize(x, out MinValue);

            //FINISHED
            //Updates graph if the optimization ends in silent mode
            if (this.RadicalWindow.RadicalVM.Mode == RefreshMode.Silent)
            {
                AppendStoredValues();
                this.RadicalVM.AutomateStepSize(true);
                this.RadicalVM.UpdateCurrentScoreDisplay();
                System.Action run = delegate ()
                {
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
                };
                Rhino.RhinoApp.MainApplicationWindow.Invoke(run);
            }
            this.RadicalWindow.OptimizationFinished();

            return result;
        }

        //Adds optimization values to array that updates graph display 
        public void AppendStoredValues()
        {
            this.RadicalVM.ObjectiveEvolution.AddRange(StoredMainValues);
            StoredMainValues.Clear();

            for (int i = 0; i < this.RadicalVM.ConstraintsEvolution.Count; i++)
            {
                this.RadicalVM.ConstraintsEvolution[i].AddRange(StoredConstraintValues[i]);
                StoredConstraintValues[i].Clear();
            }
        }

        public double constraint(double[] x, Constraint c)
        {
            for (int i = 0; i < nVars; i++)
            {
                IVariable var = Design.Variables[i];
                var.UpdateValue(x[i]);
            }
            foreach (IDesignGeometry vargeo in Design.Geometries)
            {
                vargeo.Update();
            }
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(false, Grasshopper.Kernel.GH_SolutionMode.Silent);
            return c.CurrentValue - c.LimitValue;
        }

        //UNIMPLEMENTED
        public double Objective(double[] x, ref double[] grad)
        {
            for (int i = 0; i < nVars; i++)
            {
                IVariable var = Design.Variables[i];
                var.UpdateValue(x[i]);
            }

            if (grad != null) { }//update gradient, for gradient-based algs.

            double objective = Design.Objectives[0];
            this.RadicalVM.ObjectiveEvolution.Add(objective);
            return objective;
        }
    }
}
