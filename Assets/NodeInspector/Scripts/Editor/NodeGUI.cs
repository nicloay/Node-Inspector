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
        public List<JointData> Joints;
        Rect windowRect = new Rect(100,100,200,200);
            
        public Rect WindowRect{
            get{
                return scriptableObject.EditorWindowRect;
            }
        }

        public NodeGUI(ScriptableObjectNode scriptableObject){
            this.scriptableObject = scriptableObject;
            serializedObject = new SerializedObject(scriptableObject);
            Joints = new List<JointData>();
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
					JointType jointType = GetPropertyJointType(iterator);
                    if (jointType == JointType.Nan){
                        EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);                            
                    } else {
                        EditorGUILayout.LabelField(iterator.name);
                        if (Event.current.type == EventType.Repaint){                            
                            Rect lastRect = GUILayoutUtility.GetLastRect();
                            Joints.Add(new JointData(lastRect, JointType.OneToMany_IN));
                        }
                    }
                }                    
            }
            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        JointType GetPropertyJointType(SerializedProperty property){
            if (property.propertyType != SerializedPropertyType.ObjectReference){
                return JointType.Nan;
            }
			JointAttribute joint=(JointAttribute) Attribute.GetCustomAttribute(property.serializedObject.targetObject.GetType().GetField(property.name), typeof(JointAttribute));
            if (joint == null){
                return JointType.Nan;
            } else {
                return joint.JointType;
            }

        }

    }
}