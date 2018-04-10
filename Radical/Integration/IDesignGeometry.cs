using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

namespace Radical.Integration
{
    public interface IDesignGeometry
    {
        List<IGeoVariable> Variables { get; set; }
        void VarUpdate(IGeoVariable geovar);
        void Update();
    }
}
