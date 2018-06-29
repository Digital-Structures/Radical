using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;


namespace Radical.Integration
{
    //GEO VARIABLE
    //Base class for manipulation of geometric control point variables
    //Used by SurfaceVariable, CurveVariable, and MeshVariable
    public class GeoVariable:IVariable
    {
        //CONSTRUCTOR
        public GeoVariable(double min, double max, int dir, IDesignGeometry geo)
        {
            this.dir = dir;
            this.min = min;
            this.max = max;
            this.Geometry = geo;
        }
        public IDesignGeometry Geometry { get; set; }

        //IS ACTIVE
        //Determines whether variable should be considered in optimization
        public bool IsActive
        {
            get; set;
        }

        //CURRENT VALUE
        //Position of the control point relative to its starting position
        private double value;
        public double CurrentValue
        {
            get { return this.value; }
            set { this.value = value; }
        }

        //DIRECTION
        //The direction (x,y,z) in which this point can move
        private int dir;
        public int Dir
        {
            get { return this.dir; }
            set { this.dir = value; }
        }

        //MAX
        //Maximum displacement of the control point from its original state
        private double min;
        public double Min
        {
            get { return this.min; }
            set { this.min = value; }
        }

        //MIN
        //Minimum displacement of the control point from its original state
        private double max;
        public double Max
        {
            get { return this.max; }
            set { this.max = value; }
        }

        //PARAMETER
        //Determines to what type of grasshopper object the 
        public IGH_Param Parameter
        {
            get{return ((DesignSurface)Geometry).Parameter;}
        }

        //UPDATE VALUE
        //Change variable's value and update its corresponding geometry
        public void UpdateValue(double x)
        {
            this.CurrentValue = x;
            Geometry.VarUpdate(this);
        }

        public double Gradient()
        {
            throw new NotImplementedException();
        }
    }
}
