using NodeInspector;
using System.Collections.Generic;


namespace NodeInspector.Demo.OneToMany{    
    public class SimpleEntity : ScriptableObjectNode {
        public string Name;
        [OneToMany(DirectionType.Output, "entity")]
        public List<SimpleEntity> UsedIn;
    }
}
