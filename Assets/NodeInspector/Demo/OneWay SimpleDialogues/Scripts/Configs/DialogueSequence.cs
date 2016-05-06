using UnityEngine;
using System.Collections.Generic;
using NodeInspector.Demo.OneWay;

namespace NodeInspector.Demo.Dialogue{   
    [CreateAssetMenu(menuName = "NodeInspectorDemo/Dialogue")]
    public class DialogueSequence : ScriptableObject {
        [GraphAttribute("StartItem")]
        public List<DialogueNode> Items;
        public DialogueNode StartItem;

    }
}
