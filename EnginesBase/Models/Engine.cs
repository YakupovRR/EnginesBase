using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnginesBase.Models
{
    public class Engine
    {
        public int EngineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Dictionary<Element, int> ChildElements { get; set; } = new Dictionary<Element, int>();
                

    }
}
