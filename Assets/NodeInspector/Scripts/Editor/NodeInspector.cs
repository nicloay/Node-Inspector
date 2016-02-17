using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Linq;
using NodeInspector;
using System.Collections.Generic;

namespace NodeInspector.Editor{    
    public class NodeInspector : EditorWindow {
        

        Rect defaultWindowRect = new Rect(100,100,200,200);
        NodeGUI soGUI = new NodeGUI(new ScriptableObject());
        NodeGUI soGUI2 = new NodeGUI(new ScriptableObject());
        void OnGUI(){
            if (!CheckSelectedObject()){
                return;
            }

            OnGUIToolBar();
            BeginWindows();

            soGUI.OnGUI();
            soGUI2.OnGUI();
            EndWindows();
        }

        Dictionary<string, GraphData> nodes;
        int selectedGraph;


        bool CheckSelectedObject(){
            if (Selection.activeObject == null || !(Selection.activeObject is ScriptableObject)){
                return false;
            }
            ScriptableObject so = Selection.activeObject as ScriptableObject;
            nodes = new Dictionary<string, GraphData>();
            foreach (FieldInfo fieldInfo in  so.GetType().GetFields()){
                GraphData data;
                if (GraphData.CanCreateGraphData(so, fieldInfo, out data)){
                    string uniqueName = data.PropertyName;
                    int i =0;
                    while (nodes.Keys.Contains(uniqueName)){
                        uniqueName = data.PropertyName+" ["+(++i)+"]";
                    }
                    nodes.Add(uniqueName, data);
                }
            }
            return nodes.Count > 0;
        }




        void OnGUIToolBar(){            
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);

            GUICreateMenuItems();
            GUILayout.FlexibleSpace();
            GUICreateGraphsItems();
            GUILayout.EndHorizontal();
        }

        void GUICreateMenuItems(){
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
        }


        void GUICreateGraphsItems(){            
            selectedGraph = EditorGUILayout.Popup(selectedGraph, nodes.Keys.ToArray(), EditorStyles.toolbarPopup);
        }


        [MenuItem("Test/GUIWindow")]
        static void Init(){
            
            EditorWindow.GetWindow(typeof(NodeInspector));

        }


    }    
}