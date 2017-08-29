using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp4.DataModels
{
    public class D
    {
        public List<string> EntitySets { get; set; }
    }

    public class RootObject
    {
        public D d { get; set; }
    }
}
