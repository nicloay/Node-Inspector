using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Linq;
using NodeInspector;
using System.Collections.Generic;
using System;

namespace NodeInspector.Editor{    
    public class NodeInspector : EditorWindow {
        
        Rect defaultWindowRect = new Rect(100,100,200,200);
        void OnGUI(){
            if (!CheckSelectedObject()){
                return;
            }
            List<NodeGUI> nodeGUIS  = new List<NodeGUI>();
            OnGUIToolBar();
            Rect buttonRect = new Rect();
            BeginWindows();

            foreach(var graphData in CurrentGraph.ItemList){
                NodeGUI nodeGUI = new NodeGUI((ScriptableObjectNode)graphData);
                nodeGUI.OnGUI();
                nodeGUIS.Add(nodeGUI);               
            }

            EndWindows();

            if (Event.current.type == EventType.Repaint){
                foreach (NodeGUI node in nodeGUIS){
                    RenderButtons(node, node.WindowRect);
                }
            }
        }

        static void RenderButtons(NodeGUI node, Rect WindowRect)
        {
            foreach (JoinData join in node.Joins)
            {
                Rect buttonRect = WindowRect;
                buttonRect.x += WindowRect.width;
                buttonRect.y += join.FieldInternalRect.y;
                buttonRect.width = buttonRect.height = join.FieldInternalRect.height;
                Debug.Log(buttonRect);
                if (Event.current.type == EventType.Repaint)
                {
                    GUI.Button(buttonRect, "1", EditorStyles.miniButtonRight);
                }
            }
        }

        Dictionary<string, GraphData> nodes;
        int currentGraphId;
        GraphData CurrentGraph{
            get{
                return nodes.Values.ElementAt(currentGraphId);
            }
        }


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
                if (CurrentGraph != null){
                    var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s=>s.GetTypes())
                        .Where(p=>CurrentGraph.ItemBaseType.IsAssignableFrom(p));
                    foreach (Type nodeType in types){
                        string menuPath = nodeType.Name ;
                        Type attributeType = typeof(NodeMenuItemAttribute);
                        NodeMenuItemAttribute attr = (NodeMenuItemAttribute)Attribute.GetCustomAttribute(nodeType, attributeType);
                        if (attr != null && !string.IsNullOrEmpty(attr.MenuPath)){
                            menuPath = attr.MenuPath;
                        }

                        toolsMenu.AddItem(new GUIContent(menuPath), false,  (incomeNodeType) => {
                            CurrentGraph.AddNewAsset(incomeNodeType as Type);
                        }, nodeType);

                    }
                }

                Rect rect = GUILayoutUtility.GetLastRect();
                rect.y+= EditorStyles.toolbar.fixedHeight/2.0f;
                toolsMenu.DropDown(rect);
            }
        }


        void GUICreateGraphsItems(){            
            currentGraphId = EditorGUILayout.Popup(currentGraphId, nodes.Keys.ToArray(), EditorStyles.toolbarPopup);
        }


        [MenuItem("Test/GUIWindow")]
        static void Init(){
            
            EditorWindow.GetWindow(typeof(NodeInspector));

        }


    }    
}