using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using NodeInspector;

namespace NodeInspector.Demo{    
    [CreateAssetMenu()]
    public class TestGraph : ScriptableObject {
        [Graph("TestGraph")]
        public List<CustomNode> AllNodes;

        [Graph]
        public List<CustomNode> AnotherAllNodes;

    }    
}