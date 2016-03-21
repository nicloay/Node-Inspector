using UnityEngine;
using UnityEngine.Serialization;


namespace NodeInspector {
    public class ScriptableObjectNode : ScriptableObject {        
        [HideInInspector]
        public Rect EditorWindowRect;
    }
}