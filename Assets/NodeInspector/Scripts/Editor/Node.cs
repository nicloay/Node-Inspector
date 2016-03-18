using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using NodeInspector;
using System;
using System.Runtime.Remoting.Messaging;

namespace NodeInspector.Editor {
    public class Node {
        public const int MinTopLeftDistance = 10;
        public const int KnobSize = 15;
        public static int OriginalRightPatdding {get; private set;}

        public int ControlID {get; private set;}
        public List<Joint> Joints;

        public NodeInspector ParentWindow {get; private set;}

        public Rect WindowRect{
            get{
                return scriptableObject.EditorWindowRect;
            }
        }

        ScriptableObjectNode scriptableObject;
        SerializedObject serializedObject;
        Rect windowRect = new Rect(100,100,200,200);

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

       
        public static Node GetInstance(ScriptableObjectNode scriptableObjectNode, NodeInspector parentWindow){
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Node result = (Node) GUIUtility.GetStateObject(typeof(Node), controlID);
            result.ControlID = controlID;
            result.ParentWindow = parentWindow;
            if (result.scriptableObject != scriptableObjectNode){
                result.scriptableObject = scriptableObjectNode;
                result.serializedObject = new SerializedObject(scriptableObjectNode);
                result.Joints = new List<Joint>();
            }
            return result;
        }


        public Rect OnGUI(){
            
           
            string windowName = scriptableObject.GetType().Name;
            if (!string.IsNullOrEmpty(scriptableObject.name)){
                windowName+=" | "+scriptableObject.name;
            }

            Rect newRect = 
                GUILayout.Window(scriptableObject.GetInstanceID(), 
                    scriptableObject.EditorWindowRect, DoWindow, 
                    windowName, WindowStyle);
            newRect.x = Mathf.Max(newRect.x, MinTopLeftDistance);
            newRect.y = Mathf.Max(newRect.y, MinTopLeftDistance);
            scriptableObject.EditorWindowRect = newRect;

            HandleContextClick();
            return WindowRect;
        }

        void HandleContextClick(){
            if (ParentWindow.CurrentGraph.StartNode == null){
                return; // we have just one option right now. later we would need to skip just default node 
            }
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.ContextClick && scriptableObject.EditorWindowRect.Contains(currentEvent.mousePosition)){
                GenericMenu menu = new GenericMenu() ;
                if (ParentWindow.CurrentGraph.StartNode != null){                    
                    menu.AddItem(new GUIContent( "Make Default"), false, (obj) => { 
                        ParentWindow.CurrentGraph.StartNode.objectReferenceValue = (UnityEngine.Object) obj;
                        ParentWindow.CurrentGraph.StartNode.serializedObject.ApplyModifiedProperties();
                    }, scriptableObject);                    
                }
                menu.AddItem (new GUIContent("Delete"), false, (obj)=>{                    
                    ParentWindow.CurrentGraph.RemoveElementFromList((ScriptableObject)obj);
                }, scriptableObject);

                menu.ShowAsContext();
                currentEvent.Use();
            }

        }


        void DoWindow(int id)
        {   
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            AddJointIfAcceptOneWay ();
            SerializedProperty iterator = serializedObject.GetIterator();
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

            foreach(Joint joint in Joints){
                joint.OnGUI();
            }

            if (ParentWindow.CurrentGraph.StartNode != null && ParentWindow.CurrentGraph.StartNode.objectReferenceValue == scriptableObject){
                Rect selectBox = new Rect(0,0, WindowRect.width - KnobSize, KnobSize);
                GUIStyle style = new GUIStyle(EditorStyles.miniLabel);
                style.alignment = TextAnchor.MiddleRight;
                GUI.Label(selectBox, " Start Node", style);
            }
            
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();

            GUI.DragWindow(new Rect(KnobSize, KnobSize, scriptableObject.EditorWindowRect.width - KnobSize*2, GUI.skin.window.border.top));
        }



        JointType GetPropertyJointType(SerializedProperty property){
            if (property.propertyType != SerializedPropertyType.ObjectReference){
                return JointType.Nan;
            }

            //Handle OneWay connections

            OneWayAttribute oneWay=(OneWayAttribute) Attribute.GetCustomAttribute
                (property.serializedObject.targetObject.GetType().GetField(property.name), typeof(OneWayAttribute));
            if (oneWay == null){
                return JointType.Nan;
            } else {
                return JointType.OneWay_OUT;
            }
        }

		void AddJointIfAcceptOneWay ()
		{
            OneWayAttribute oneWay = (OneWayAttribute)Attribute.GetCustomAttribute (scriptableObject.GetType (), typeof(OneWayAttribute));
			if (oneWay != null) {
                Joint jData = Joint.GetInstance(scriptableObject, windowRect, JointType.OneWay_IN, scriptableObject.EditorWindowRect.position, this);
                if (!Joints.Contains(jData))
                {
                    Joints.Add(jData);
                }
			}
		}

               
        void MakeKnob(SerializedProperty serializedProperty, Rect lastRect, JointType jointType)
        {
            serializedProperty = serializedProperty.serializedObject.FindProperty(serializedProperty.propertyPath);
            Joint jData = Joint.GetInstance(serializedProperty, lastRect, jointType, scriptableObject.EditorWindowRect.position, this);
            if (!Joints.Contains(jData))
            {
                Joints.Add(jData);
            }
        }
    }
}