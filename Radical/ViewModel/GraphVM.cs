using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Radical
{
    class GraphVM : BaseVM
    {
        private String linegraph_name;
        public String LineGraphName
        {
            get
            { return linegraph_name;}
            set
            {
                if (CheckPropertyChanged<String>("LineGraphName", ref linegraph_name, ref value))
                {
                    linegraph_name = value;
                }
            }
            
    }
    }
}
