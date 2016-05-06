using System;


namespace NodeInspector{    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class OneToManyAttribute : Attribute {
        public DirectionType Direction;
        public string Key;
        public OneToManyAttribute(DirectionType direction, string key){
            Direction = direction;
            Key = key;
        }
    }
}
