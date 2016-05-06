using NodeInspector;
using System.Collections.Generic;
using UnityEngine;


namespace NodeInspector.Demo.OneToMany{    
    public class SimpleEntity : ScriptableObjectNode {
        public string Name;
        [OneToMany(DirectionType.Output, "entity")]
        [SerializeField]
        public List<Part> UsedIn;
    }
}
