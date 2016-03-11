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

        public const int KnobSize = 15;
        public static int OriginalRightPatdding {get; private set;}
        static GUIStyle _windowStyle;
        static GUIStyle WindowStyle{
            get{
                if (_windowStyle == null){
                    _windowStyle = new GUIStyle(GUI.skin.window);
                    _windowStyle.overflow.left  -= KnobSize;
                    _windowStyle.overflow.right -= KnobSize;
                    _windowStyle.overflow.top   -= KnobSize;
                    OriginalRightPatdding = _windowStyle.padding.right;
                    _windowStyle.padding.left  += KnobSize;
                    _windowStyle.padding.right += KnobSize;
                    _windowStyle.padding.top   += KnobSize;
                }
                return _windowStyle;
            }
        }


        public Rect WindowRect{
            get{
                return scriptableObject.EditorWindowRect;
            }
        }

        public static NodeGUI GetInstance(ScriptableObjectNode scriptableObjectNode){
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            NodeGUI result = (NodeGUI) GUIUtility.GetStateObject(typeof(NodeGUI), controlID);
            result.ControlID = controlID;
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
                    scriptableObject.name, WindowStyle);   
            return WindowRect;
        }

        void DoWindow(int id)
        {          
            if (Event.current.type == EventType.Layout){
                Joints.Clear();
            }     


            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            SerializedProperty iterator = serializedObject.GetIterator();
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
                        MakeKnob(iterator, lastRect, jointType);
                    }
                }                    
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
            AddJointIfAcceptIncognito ();
            GUI.DragWindow();
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
                JointData jData = JointData.GetInstance(scriptableObject, windowRect, JointType.Incognito_In);
                if (!Joints.Contains(jData))
                {
                    Joints.Add(jData);
                }
                jData.OnGUI();
			}
		}

               
        void MakeKnob(SerializedProperty serializedProperty, Rect lastRect, JointType jointType)
        {
            serializedProperty = serializedProperty.serializedObject.FindProperty(serializedProperty.propertyPath);
            JointData jData = JointData.GetInstance(serializedProperty, lastRect, jointType);
            if (!Joints.Contains(jData))
            {
                Joints.Add(jData);
            }
            jData.OnGUI();
        }
    }
}