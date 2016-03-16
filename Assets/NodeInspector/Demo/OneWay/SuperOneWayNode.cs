using System.Collections.Generic;
using System;

namespace NodeInspector.Demo.OneWay{    
    [Serializable]
    public class NodeHolder{
        [OneWay]
        public OneWayNode Node;
    }

    public class SuperOneWayNode : OneWayNode {
        [OneWay]
        public SuperOneWayNode AlternativeNode;


        public List<NodeHolder> NodeHolders;
    }
}


