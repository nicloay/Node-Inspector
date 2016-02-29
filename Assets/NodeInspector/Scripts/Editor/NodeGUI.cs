using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NodeInspector.Editor {
    public class NodeGUI {
        ScriptableObjectNode scriptableObject;
        SerializedObject serializedObject;
        Rect windowRect = new Rect(100,100,200,200);
            
        public NodeGUI(ScriptableObjectNode scriptableObject){
            this.scriptableObject = scriptableObject;
            serializedObject = new SerializedObject(scriptableObject);
        }


        public void OnGUI(){
            scriptableObject.EditorWindowPosition = 
                GUILayout.Window(scriptableObject.GetInstanceID(), 
                    scriptableObject.EditorWindowPosition, DoWindow, 
                    "scriptableObject.name");   
        }

        void DoWindow(int id)
        {
            DoDrawDefaultInspector(serializedObject);
            GUI.DragWindow();
        }


       
        static bool DoDrawDefaultInspector(SerializedObject obj)
        {
            EditorGUI.BeginChangeCheck();
            obj.Update();
            SerializedProperty iterator = obj.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false){
                if (iterator.propertyType != SerializedPropertyType.ObjectReference || !(iterator.objectReferenceValue is MonoScript)){                    
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                    //Check if it's node here
                    if (iterator.propertyType == SerializedPropertyType.ObjectReference){
                        
                    }
                }                    
            }
            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }
    }
}