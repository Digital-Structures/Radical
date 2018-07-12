using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Radical.Components;

namespace Radical.Integration
{
    public interface IDesign
    {
        // Ok
        double CurrentScore { get; set; }
        List<IVariable> Variables { get; set; }
        List<IVariable> ActiveVariables { get; }
        List<IDesignGeometry> Geometries { get; set; }
        List<Constraint> Constraints { get; }
        void Optimize();
        void Sample(int alg);


        //List<double> Properties { get; set; }

        // Bad
        List<List<double>> Samples {get;set;}
        List<List<double>> Properties { get; set; }
        List<double> ScoreEvolution { get; set; }
        List<List<double>> ConstraintEvolution { get; set; }

        void Optimize(RadicalWindow radicalWindow);
        
        // to remove if possible
        IGH_Param ScoreParameter { get; set; }
        // to remove if possible
        IOptimizationComponent OptComponent { get; set; }
    }
}
