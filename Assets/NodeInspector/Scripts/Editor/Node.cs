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
//                    _windowStyle.overflow.top   -= KnobSize;
                    OriginalRightPatdding = _windowStyle.padding.right;
                    _windowStyle.padding.left  += KnobSize;
                    _windowStyle.padding.right += KnobSize;
//                    _windowStyle.padding.top   += KnobSize;
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


        public void OnGUI(){            
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
        }

        void HandleContextClick(){
            if (ParentWindow.CurrentGraph.StartNode == null){
                return; // we have just one option right now. later we would need to skip just default node 
            }
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.ContextClick && scriptableObject.EditorWindowRect.Contains(currentEvent.mousePosition)){
                ShowNodeContext();
            }
        }

        HashSet<Joint> currentJoints =new HashSet<Joint>();
        void DoWindow(int id)
        {   
            EditorGUI.BeginChangeCheck();
            currentJoints.Clear();
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

            RemoveUnusedJoints(Joints);

            foreach(Joint joint in Joints){
                joint.OnGUI();
            }

            if (ParentWindow.CurrentGraph.StartNode != null && ParentWindow.CurrentGraph.StartNode.objectReferenceValue == scriptableObject){
                Rect selectBox = new Rect(0,WindowRect.height - KnobSize, WindowRect.width - KnobSize, KnobSize);
                GUIStyle style = new GUIStyle(EditorStyles.miniLabel);
                style.alignment = TextAnchor.MiddleRight;
                GUI.Label(selectBox, " Start Node", style);
            }

            ShowWindowMenu();
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();

            GUI.DragWindow(new Rect(KnobSize, 0, scriptableObject.EditorWindowRect.width - KnobSize*2, GUI.skin.window.border.top));
        }


        void ShowWindowMenu(){
            GUIStyle style = GUI.skin.GetStyle("PaneOptions");
            Rect rect = new Rect(WindowRect.width - KnobSize - 20, 5, 20,20);

            if (GUI.Button(rect , "", style)){
                ShowNodeContext();
            }
        }

        void RemoveUnusedJoints(List<Joint> joints)
        {
            List<Joint> jointsToRemove = new List<Joint>();
            foreach(var joint in joints){
                if (!currentJoints.Contains(joint)){
                    jointsToRemove.Add(joint);
                }
            }
            foreach (var joint in jointsToRemove){
                joints.Remove(joint);
            }
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
                currentJoints.Add(jData);
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
            currentJoints.Add(jData);
        }

        void ShowNodeContext()
        {
            Event currentEvent = Event.current;
            GenericMenu menu = new GenericMenu();
            if (ParentWindow.CurrentGraph.StartNode != null)
            {
                menu.AddItem(new GUIContent("StartNode"), false, obj => 
                {
                    ParentWindow.CurrentGraph.StartNode.objectReferenceValue = (UnityEngine.Object)obj;
                    ParentWindow.CurrentGraph.StartNode.serializedObject.ApplyModifiedProperties();
                }, scriptableObject);
            }
            menu.AddItem(new GUIContent("Delete"), false, obj => 
            {
                ParentWindow.CurrentGraph.RemoveElementFromList((ScriptableObject)obj);
            }, scriptableObject);
            menu.ShowAsContext();
            currentEvent.Use();
        }
    }
}