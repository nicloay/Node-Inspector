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
			AddJointIfAcceptIncognito ();
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
                        Rect lastRect = GUILayoutUtility.GetLastRect();
						Joints.Add(new JointData(iterator, lastRect, scriptableObject.EditorWindowRect, jointType));
                    
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

		void AddJointIfAcceptIncognito ()
		{
			JointAttribute joint = (JointAttribute)Attribute.GetCustomAttribute (scriptableObject.GetType (), typeof(JointAttribute));
			if (joint != null) {
				Joints.Add (new JointData (scriptableObject, windowRect, scriptableObject.EditorWindowRect, JointType.Incognito_In));
			}
		}
    }
}