using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NodeInspector.Editor{    
    public class NodeInspector : EditorWindow {
        

        Rect defaultWindowRect = new Rect(100,100,200,200);
        NodeGUI soGUI = new NodeGUI(new ScriptableObject());
        NodeGUI soGUI2 = new NodeGUI(new ScriptableObject());
        void OnGUI(){
            OnGUIToolBar();
            BeginWindows();

            soGUI.OnGUI();
            soGUI2.OnGUI();
            EndWindows();
        }
            


        void OnGUIToolBar(){            
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            if (GUILayout.Button("Create",EditorStyles.toolbarDropDown, new GUILayoutOption[0]))
            {                
                GenericMenu toolsMenu = new GenericMenu();

                toolsMenu.AddItem(new GUIContent("Menu1"), false, null);
            
                toolsMenu.AddDisabledItem(new GUIContent("Menu2"));
                toolsMenu.AddSeparator("");
                toolsMenu.AddItem(new GUIContent("Help..."), false, null);
                // Offset menu from right of editor window

                Rect rect = GUILayoutUtility.GetLastRect();
                rect.y+= EditorStyles.toolbar.fixedHeight/2.0f;
                toolsMenu.DropDown(rect);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }



        [MenuItem("Test/GUIWindow")]
        static void Init(){
            
            EditorWindow.GetWindow(typeof(NodeInspector));

        }


    }    
}