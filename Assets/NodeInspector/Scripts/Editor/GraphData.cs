using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;
using System.Linq;
using UnityEditor;

namespace NodeInspector.Editor{
    
    public class GraphData {
        public ScriptableObject ParentObject;
        public Type ItemBaseType{get; private set;}
        public String PropertyName;
        public IList  ItemList;
        public SerializedProperty StartNode;
        public SerializedProperty SerializedItemList;

        private GraphData()
        {            
        }


        public void AddNewAsset(Type nodeType){            
            ScriptableObject instance = ScriptableObject.CreateInstance(nodeType);
            AssetDatabase.AddObjectToAsset(instance, ParentObject);
            instance.hideFlags = HideFlags.HideInHierarchy;
            ItemList.Add(instance);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ParentObject));
        }

        public static bool CanCreateGraphData(ScriptableObject parentObject, FieldInfo fieldInfo, out GraphData graphData){
            graphData = null;          
            Type fieldValueType = fieldInfo.FieldType;
            if (fieldValueType.IsGenericType && (fieldValueType.GetGenericTypeDefinition() == typeof(List<>))
                && typeof(ScriptableObject).IsAssignableFrom( fieldValueType.GetGenericArguments()[0])){

                object[] attributes = fieldInfo.GetCustomAttributes(false);
                if (attributes == null || attributes.Length == 0){
                    return false;
                }
                GraphAttribute attribute =  attributes
                    .ToList().First((arg) => arg.GetType() == typeof(GraphAttribute)) as GraphAttribute;
                if (attribute != null){
                    object fieldValue = fieldInfo.GetValue(parentObject);
                    if (fieldValue == null){                        
                        var newList = Activator.CreateInstance(fieldValueType);
                        fieldInfo.SetValue(parentObject, newList);
                        fieldValue = newList;
                    }
                    SerializedObject serializedObject = new SerializedObject(parentObject);
                    graphData = new GraphData();
                    graphData.ItemBaseType = fieldValueType.GetGenericArguments()[0];
                    graphData.ItemList = fieldValue as IList;
                    graphData.PropertyName = fieldInfo.Name;
                    graphData.ParentObject = parentObject;
                    graphData.SerializedItemList = serializedObject.FindProperty(fieldInfo.Name);
                    if (string.IsNullOrEmpty(graphData.PropertyName)){
                        graphData.PropertyName = fieldInfo.Name;
                    }
                    graphData.StartNode = null;
                    if (!string.IsNullOrEmpty(attribute.StartNode)){
                        graphData.StartNode = serializedObject.FindProperty(attribute.StartNode);
                        if (graphData.StartNode == null){
                            Debug.LogError("Cant find property with name " + attribute.StartNode +" for this graph");
                        } else if (false){ //fixme through reflexion get field type
                            graphData.StartNode = null ;
                            Debug.LogError("Start node type is not assignable from graph node type");
                        }

                    } 
                    graphData.SetDefaultStartNodeIfNothingSelected();
                    return true;
                }
            }


            return false;
        }

        void SetDefaultStartNodeIfNothingSelected(){
            if (StartNode != null && StartNode.objectReferenceValue == null && ItemList.Count > 0){
                StartNode.objectReferenceValue = ItemList[0] as UnityEngine.Object;
                StartNode.serializedObject.ApplyModifiedProperties();
            }
        }

        public void RemoveElementFromList(ScriptableObject listItem){
            for (int index = 0; index < ItemList.Count; index++) {
                if (ItemList[index] == listItem){                            
                    SerializedItemList.GetArrayElementAtIndex(index).objectReferenceValue = null;
                    SerializedItemList.DeleteArrayElementAtIndex(index);
                    SerializedItemList.serializedObject.ApplyModifiedProperties();
                    break;
                }
            }

            if (StartNode != null 
                && StartNode.objectReferenceValue == listItem){
                StartNode.objectReferenceValue = null;
                StartNode.serializedObject.ApplyModifiedProperties();
            }

            //FIXME: unfortunatly we can't remove this subasset from main asset because we will lost
            //        this object and won't be able to Undo this action. Try to fix that later and clean junk on some events like window close.
	    Undo.DestroyObjectImmediate(listItem);
	    GameObject.DestroyImmediate(listItem, true);
        }

    }
}
