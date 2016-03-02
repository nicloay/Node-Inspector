using System;

namespace NodeInspector{    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]

    public class JoinAttribute : Attribute {
        public JoinType JoinType;

        public JoinAttribute(JoinType joinType){
            JoinType = joinType;
        }
    }
}
