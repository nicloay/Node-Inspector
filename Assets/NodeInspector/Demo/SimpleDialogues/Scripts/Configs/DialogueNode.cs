using UnityEngine;
using System.Collections;

namespace NodeInspector.Demo.Dialogue{
    [OneWay]
    public abstract class DialogueNode : ScriptableObjectNode {
        public abstract void Execute(GameObject actor, GameObject opponent);
    }    
}
