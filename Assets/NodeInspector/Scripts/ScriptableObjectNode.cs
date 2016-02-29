using UnityEngine;


namespace NodeInspector {
    public class ScriptableObjectNode : ScriptableObject {
        [HideInInspector]
        public Rect EditorWindowPosition;
    }
}