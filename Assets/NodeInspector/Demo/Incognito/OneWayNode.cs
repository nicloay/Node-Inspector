using UnityEngine;

using NodeInspector;


namespace NodeInspector.Demo{    
	[OneWay]
    public class OneWayNode : ScriptableObjectNode {
		[OneWay]
        public OneWayNode NextNode;
        public OneWayNode UnusedNode;
    }    
}