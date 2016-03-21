using System;

namespace NodeInspector{    
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeMenuItemAttribute : Attribute {
        public string MenuPath;
    	
        public NodeMenuItemAttribute(String menuPath){
            this.MenuPath = menuPath;
        }
    }
}