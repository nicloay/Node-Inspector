using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System.Linq;
using NodeInspector;
using System.Collections.Generic;
using System;

namespace NodeInspector.Editor{    
    public class JointHighlightHolder{
        public JointType JointType;
    }


    public class NodeInspector : EditorWindow {
        public int ControlID{get; private set;}
        public JointHighlightHolder JointHighlight;

        void OnGUI(){
            
            JointHighlight = (JointHighlightHolder) GUIUtility.GetStateObject(typeof(JointHighlightHolder)
                ,GUIUtility.GetControlID(FocusType.Passive));
            
            if (!CheckSelectedObject()){
                return;
            }

            if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed"){
                Event.current.Use();
            }

            List<Node> nodeGUIS  = new List<Node>();
            OnGUIToolBar();

            foreach(var graphData in CurrentGraph.ItemList){
                Node nodeGUI = Node.GetInstance((ScriptableObjectNode)graphData, this);               
                nodeGUIS.Add(nodeGUI);               
            }

            BeginWindows();
            foreach (Node node in nodeGUIS){
                node.OnGUI();
            }
            EndWindows();

            ConnectionsCollection cCollection = new ConnectionsCollection (nodeGUIS, this);

            foreach (Connection connectionGUI in cCollection.allConnections) {
                connectionGUI.OnGUI();
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

       
        [MenuItem("Test/GUIWindow")]
        static void Init(){
            
            EditorWindow.GetWindow(typeof(NodeInspector));

        }



    }    
}