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

            foreach(var graphData in CurrentGraph.ItemList){
                NodeGUI nodeGUI = NodeGUI.GetInstance(GUIUtility.GetControlID(FocusType.Passive), (ScriptableObjectNode)graphData);               
                nodeGUIS.Add(nodeGUI);               
            }

            BeginWindows();
            foreach (NodeGUI node in nodeGUIS){
                node.OnGUI();
            }
            EndWindows();
            
            foreach (NodeGUI node in nodeGUIS){
                RenderButtons(node, node.WindowRect);
            }
            ConnectionsCollection cCollection = new ConnectionsCollection (nodeGUIS);

            Debug.Log(Event.current.type + "  " + cCollection.allConnections.Count);
            foreach (ConnectionGUI connectionGUI in cCollection.allConnections) {
                connectionGUI.OnGUI();
			}

            //Debug.Log("" + GUIUtility.hotControl);
            //HandleKeyDown();
        }

        static void RenderButtons(NodeGUI node, Rect WindowRect)
        {
			foreach (JointData joint in node.Joints)
            {                
                if (Event.current.type == EventType.Repaint)
                {
                    GUI.Button(joint.KnobButtonRect, joint.KnobButtonCaption, joint.KnobButtonStyle);
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


        //FIXME: show only nodes accepted by current graph
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

        void HandleKeyDown()
        {
            if (Event.current.isMouse && Event.current.button == 0){
                
            }

            if (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace){
                
                Debug.LogFormat("delete pressed object = {0}", GUIUtility.GetControlID(FocusType.Passive));
            }
        }

        [MenuItem("Test/GUIWindow")]
        static void Init(){
            
            EditorWindow.GetWindow(typeof(NodeInspector));

        }



    }    
}