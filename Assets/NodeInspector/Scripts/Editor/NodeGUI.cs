using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using NodeInspector;
using System;

namespace NodeInspector.Editor {
    public class NodeGUI {
        public int ControlID {get; private set;}
        ScriptableObjectNode scriptableObject;
        SerializedObject serializedObject;
        public List<JointData> Joints;
        Rect windowRect = new Rect(100,100,200,200);
            
        public Rect WindowRect{
            get{
                return scriptableObject.EditorWindowRect;
            }
        }

        public static NodeGUI GetInstance(int ControlID, ScriptableObjectNode scriptableObjectNode){
            NodeGUI result = (NodeGUI) GUIUtility.GetStateObject(typeof(NodeGUI), ControlID);
            if (result.scriptableObject != scriptableObjectNode){
                result.scriptableObject = scriptableObjectNode;
                result.serializedObject = new SerializedObject(scriptableObjectNode);
                result.Joints = new List<JointData>();
            }
            return result;
        }


        public Rect OnGUI(){
            scriptableObject.EditorWindowRect = 
                GUILayout.Window(scriptableObject.GetInstanceID(), 
                    scriptableObject.EditorWindowRect, DoWindow, 
                    scriptableObject.name);   
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
            bool fillJointCollection = (Joints.Count == 0);
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false){
                if (iterator.propertyType != SerializedPropertyType.ObjectReference || !(iterator.objectReferenceValue is MonoScript)){                    
                    //Check if it's node here
					JointType jointType = GetPropertyJointType(iterator);
                    if (jointType == JointType.Nan){
                        EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);                            
                    } else {
                        EditorGUILayout.LabelField(iterator.name);
                                                 
                        Rect lastRect = GUILayoutUtility.GetLastRect();                            
                        AddJointDataOrUpdate(iterator.objectReferenceValue, lastRect, jointType);
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
                AddJointDataOrUpdate(scriptableObject, windowRect, JointType.Incognito_In);
				//Joints.Add (new JointData (scriptableObject, windowRect, scriptableObject.EditorWindowRect, JointType.Incognito_In));
			}
		}

               
        void AddJointDataOrUpdate(UnityEngine.Object objectRefferencevalue, Rect lastRect, JointType jointType)
        {
            JointData jData = JointData.GetInstance(GUIUtility.GetControlID(FocusType.Passive), objectRefferencevalue, lastRect, scriptableObject.EditorWindowRect, jointType);
            if (!Joints.Contains(jData))
            {
                Joints.Add(jData);
            }
        }
    }
}