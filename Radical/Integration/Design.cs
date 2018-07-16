using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Radical.Components;
using Radical.TestComponents;


namespace Radical.Integration
{
    //DESIGN
    //Collection of problem variables, constraints, and objectives to be optimized by Radical
    public class Design : IDesign
    {
        //Radical Component
        public Design(List<IVariable> vars, RadicalComponent component)
        {
            this.Variables = vars;
           // this.ScoreEvolution = new List<double>();
            this.OptComponent = component;
        }

        public Design(List<IVariable> vars, List<Constraint> consts, RadicalComponent component)
        {
            this.Variables = vars;
            this.Constraints = consts;
          //  this.ScoreEvolution = new List<double>();
            this.OptComponent = component;
        }

        //CONSTRUCTOR
        public Design(List<IVariable> vars, List<IDesignGeometry> geos, List<Constraint> consts, RadicalComponent component)
        {
            this.Variables = vars;
            this.Constraints = consts;
            this.Geometries = geos;
            if (Geometries.Any()) { this.Variables.AddRange(Geometries.Select(x => x.Variables).SelectMany(x => x).ToList()); } // not the cleanest way to do it, review code structure
            this.ScoreEvolution = new List<double>();
            this.ConstraintEvolution = new List<List<double>>();
            foreach (Constraint c in this.Constraints)
            {
                this.ConstraintEvolution.Add(new List<double>());
            }


            this.OptComponent = component;
        }

        //EVALUATION PROPERTIES
        public List<List<double>> Samples { get; set; }
        public List<List<double>> Properties { get; set; }
        public IOptimizationComponent OptComponent { get; set; }
        public IExplorationComponent ExpComponent;
        public List<double> ScoreEvolution { get; set; }
        public List<List<double>> ConstraintEvolution { get; set; }
        public IGH_Param ScoreParameter { get; set; }

        //INPUT PROPERTIES
        public List<IVariable> Variables { get; set; }
        public List<IVariable> ActiveVariables { get { return Variables.Where(var => var.IsActive).ToList(); } }
        public List<IDesignGeometry> Geometries { get; set; }
        public List<Constraint> Constraints { get; set; }
        public List<Constraint> ActiveConstraints { get { return Constraints.Where(c => c.IsActive).ToList(); } }
        public List<double> ConstraintsNumber
        {
            get { return OptComponent.Constraints; }
        }


        //CURRENT SCORE
        //The value of the objective with the current variable values
        public double CurrentScore
        {
            get
            {
                return OptComponent.Objective;
            }
            set { }
        }

        public void Optimize()
        {
            Optimizer opt = new Optimizer(this);
            opt.RunOptimization();
            this.OptComponent.Evolution = this.ScoreEvolution;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
        }

        //OPTIMIZE for Radical
        //Runs the optimizer and stores the objective data
        public void Optimize(RadicalWindow radicalWindow)
        {
            Optimizer opt = new Optimizer(this, radicalWindow);
            opt.RunOptimization();
            this.OptComponent.Evolution = this.ScoreEvolution;
        }

        //SAMPLE
        public void Sample(int alg)
        {

            Sampler.ISamplingAlg samplingAlg;
            switch (alg)
            {
                case 0:
                    samplingAlg = new Sampler.GSampling();
                    break;
                case 1:
                    samplingAlg = new Sampler.RUSampling();
                    break;
                case 2:
                    samplingAlg = new Sampler.LHSampling();
                    break;
                default:
                    samplingAlg = new Sampler.LHSampling();
                    break;
            }
            Sampler sam = new Sampler(this, samplingAlg, ExpComponent.nSamples);
            sam.RunSampling();
        }

        #region obsolete_constructors
        public Design(List<IVariable> vars, IExplorationComponent component)
        {
            this.Variables = vars;
            //this.CurrentScore = 0; //init objective function
            this.Samples = new List<List<double>>();
            this.Properties = new List<List<double>>();
            this.ExpComponent = component;
        }

        public Design(List<IVariable> vars, IOptimizationComponent component)
        {
            this.Variables = vars;
            //this.CurrentScore = 0; //init objective function
            this.ScoreEvolution = new List<double>();
            this.OptComponent = component;
        }

        public Design(List<IVariable> vars, List<IDesignGeometry> geos, IOptimizationComponent component)
        {
            this.Variables = vars;
            this.Geometries = geos;
            if (Geometries.Any()) { this.Variables.AddRange(Geometries.Select(x => x.Variables).SelectMany(x => x).ToList()); } // not the cleanest way to do it, review code structure
            //this.CurrentScore = 0; //init objective function
            this.ScoreEvolution = new List<double>();
            this.OptComponent = component;
        }
        #endregion
    }
}
