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
            object fieldValue = fieldInfo.GetValue(parentObject);
            if (fieldValue == null){
                return false;
            }
            Type fieldValueType = fieldValue.GetType();
            if (fieldValueType.IsGenericType && (fieldValueType.GetGenericTypeDefinition() == typeof(List<>))
                && typeof(ScriptableObject).IsAssignableFrom( fieldValueType.GetGenericArguments()[0])){

                object[] attributes = fieldInfo.GetCustomAttributes(false);
                if (attributes == null || attributes.Length == 0){
                    return false;
                }
                GraphAttribute attribute =  attributes
                    .ToList().First((arg) => arg.GetType() == typeof(GraphAttribute)) as GraphAttribute;
                if (attribute != null){
                    graphData = new GraphData();
                    graphData.ItemBaseType = fieldValueType.GetGenericArguments()[0];
                    graphData.ItemList = fieldValue as IList;
                    graphData.PropertyName = attribute.StartNode;
                    graphData.ParentObject = parentObject;

                    if (string.IsNullOrEmpty(graphData.PropertyName)){
                        graphData.PropertyName = fieldInfo.Name;
                    }
                    graphData.StartNode = null;
                    if (!string.IsNullOrEmpty(attribute.StartNode)){
                        graphData.StartNode = (new SerializedObject(parentObject)).FindProperty(attribute.StartNode);
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

    }
}
