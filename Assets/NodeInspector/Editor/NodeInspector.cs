using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NodeInspector{    
    public class NodeInspector : EditorWindow {

        Rect defaultWindowRect = new Rect(100,100,200,200);

        void OnGUI(){
            BeginWindows();

            defaultWindowRect = GUILayout.Window(1,defaultWindowRect, DoWindow, "hey");
            EndWindows();
        }

        void DoWindow(int id)
        {
            GUILayout.Button("hi");
            GUI.DragWindow();
        }

        [MenuItem("Test/GUIWindow")]
        static void Init(){
            EditorWindow.GetWindow(typeof(NodeInspector));
        }
    }    
}