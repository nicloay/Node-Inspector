using UnityEngine;
using System.Collections.Generic;


namespace NodeInspector.Demo.OneWay{  
    [CreateAssetMenu]
    public class TestOneWayNodeGraph : ScriptableObject {
        [Graph("StartNode")]
        public List<OneWayNode> OneWayNodes;

        public OneWayNode StartNode;
    }    
}