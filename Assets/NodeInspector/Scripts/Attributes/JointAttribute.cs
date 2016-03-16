using System;

namespace NodeInspector{    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]

    public class JointAttribute : Attribute {
        //public JointType JointType;

        public JointAttribute(){
          //  JointType = jointType;
        }
    }
}
