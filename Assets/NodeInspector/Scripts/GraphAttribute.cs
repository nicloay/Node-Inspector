using UnityEngine;
using System.Collections.Generic;
using System;

namespace NodeInspector{    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class GraphAttribute : System.Attribute {
        public String Name;
        public GraphAttribute() : this (""){            
        }

        public GraphAttribute(string name){
            Name = name;
        }
    }
}
