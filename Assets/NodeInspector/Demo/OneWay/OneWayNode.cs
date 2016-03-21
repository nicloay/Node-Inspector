using UnityEngine;

using NodeInspector;


namespace NodeInspector.Demo.OneWay{    
    public enum TestEnum{
        a,
        b,
        c,
        d
    }

	[OneWay]
    public class OneWayNode : ScriptableObjectNode {
        public TestEnum TestEnum;
        [OneWay]
        public OneWayNode NextNode;
        [OneWay]
        public OneWayNode NextNode2;
        [OneWay]
        public OneWayNode NextNode3;
        public OneWayNode UnusedNode;
        public int testInt;
    }    
}