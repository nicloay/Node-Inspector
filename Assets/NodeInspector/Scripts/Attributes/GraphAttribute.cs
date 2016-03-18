using UnityEngine;
using System.Collections.Generic;
using System;

namespace NodeInspector{    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class GraphAttribute : System.Attribute {
        public String StartNode;
        public GraphAttribute() : this (""){            
        }

        public GraphAttribute(string StartNode){
            this.StartNode = StartNode;
        }
    }
}
