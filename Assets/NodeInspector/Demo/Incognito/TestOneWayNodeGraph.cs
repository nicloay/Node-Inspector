using UnityEngine;
using System.Collections.Generic;


namespace NodeInspector.Demo{  
    [CreateAssetMenu]
    public class TestOneWayNodeGraph : ScriptableObject {
        [Graph]
        public List<OneWayNode> OneWayNodes;
    }    
}