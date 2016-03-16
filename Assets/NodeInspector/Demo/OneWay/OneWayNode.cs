using UnityEngine;

using NodeInspector;


namespace NodeInspector.Demo.OneWay{    
	[OneWay]
    public class OneWayNode : ScriptableObjectNode {
		[OneWay]
        public OneWayNode NextNode;
        public OneWayNode UnusedNode;
        public int testInt;
    }    
}