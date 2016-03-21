using System;

namespace NodeInspector{    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class OneWayAttribute : Attribute {        
    }
}
