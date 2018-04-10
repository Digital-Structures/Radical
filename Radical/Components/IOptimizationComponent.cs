using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radical.Components
{
    public interface IOptimizationComponent
    {
        double Objective { get; set; }
        List<double> Constraints { get; set; }
        List<double> Evolution { get; set; }
    }
}
