using System;

namespace NodeInspector{ 
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class ShowAsPropertyAttribute : Attribute {
        public Type ParentType;

        public ShowAsPropertyAttribute()
        {            
        }

        public ShowAsPropertyAttribute(Type parentType)
        {            
            ParentType = parentType;
        }
    }
}