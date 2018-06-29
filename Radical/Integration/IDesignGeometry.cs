using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

namespace Radical.Integration
{
    public interface IDesignGeometry
    {
        List<GeoVariable> Variables { get; set; }
        void VarUpdate(GeoVariable geovar);
        void Update();
    }
}
