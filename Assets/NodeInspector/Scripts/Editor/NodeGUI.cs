using UnityEngine;
using System.Collections;

namespace NodeInspector.Editor {
    public class NodeGUI {
        ScriptableObject scriptableObject;
        Rect windowRect = new Rect(100,100,200,200);
            
        public NodeGUI(ScriptableObject scriptableObject){
            this.scriptableObject = scriptableObject;
        }


        public void OnGUI(){
            windowRect = GUILayout.Window(scriptableObject.GetInstanceID(),windowRect, DoWindow, "hey");   
        }

        void DoWindow(int id)
        {
            GUILayout.Button("hi");
            GUI.DragWindow();
        }
    }
}