using NodeInspector;
using System.Collections.Generic;
using UnityEngine;


namespace NodeInspector.Demo.OneToMany{    
    public class SimpleItem : ScriptableObjectNode {
        public string Name;
        [OneToMany(DirectionType.Output, "entity")]
        public List<ComplexItem> UsedIn;
    }
}
