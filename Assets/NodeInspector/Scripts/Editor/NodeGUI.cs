using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using NodeInspector;
using System;

namespace NodeInspector.Editor {
    public class NodeGUI {
        ScriptableObjectNode scriptableObject;
        SerializedObject serializedObject;
        public List<JoinData> Joins;
        Rect windowRect = new Rect(100,100,200,200);
            
        public Rect WindowRect{
            get{
                return scriptableObject.EditorWindowRect;
            }
        }

        public NodeGUI(ScriptableObjectNode scriptableObject){
            this.scriptableObject = scriptableObject;
            serializedObject = new SerializedObject(scriptableObject);
            Joins = new List<JoinData>();
        }


        public Rect OnGUI(){
            scriptableObject.EditorWindowRect = 
                GUILayout.Window(scriptableObject.GetInstanceID(), 
                    scriptableObject.EditorWindowRect, DoWindow, 
                    "scriptableObject.name");   
            return WindowRect;
        }

        void DoWindow(int id)
        {            
            DoDrawDefaultInspector(serializedObject);
            GUI.DragWindow();
        }


       
        bool DoDrawDefaultInspector(SerializedObject obj)
        {
            EditorGUI.BeginChangeCheck();
            obj.Update();
            SerializedProperty iterator = obj.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false){
                if (iterator.propertyType != SerializedPropertyType.ObjectReference || !(iterator.objectReferenceValue is MonoScript)){                    
                    //Check if it's node here
                    JoinType joinType = GetPropertyJoinType(iterator);
                    if (joinType == JoinType.Nan){
                        EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);                            
                    } else {
                        EditorGUILayout.LabelField(iterator.name);
                        if (Event.current.type == EventType.Repaint){                            
                            Rect lastRect = GUILayoutUtility.GetLastRect();
                            Joins.Add(new JoinData(lastRect, JoinType.OneToMany_IN));
                        }
                    }
                }                    
            }
            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        JoinType GetPropertyJoinType(SerializedProperty property){
            if (property.propertyType != SerializedPropertyType.ObjectReference){
                return JoinType.Nan;
            }
            JoinAttribute join =(JoinAttribute) Attribute.GetCustomAttribute(property.serializedObject.targetObject.GetType().GetField(property.name), typeof(JoinAttribute));
            if (join == null){
                return JoinType.Nan;
            } else {
                return join.JoinType;
            }

        }

    }
}