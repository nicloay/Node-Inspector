using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace NodeInspector.Editor{
    
    public class GraphData {
        public ScriptableObject ParentObject;
        public Type ItemBaseType{get; private set;}
        public String PropertyName;
        public List<ScriptableObject>   ItemList;

        private GraphData()
        {            
        }

        public static bool CanCreateGraphData(ScriptableObject parentObject, FieldInfo fieldInfo, out GraphData graphData){
            graphData = null;
            object value = fieldInfo.GetValue(parentObject);
            Type valueType = value.GetType();
            if (valueType.IsGenericType && (valueType.GetGenericTypeDefinition() == typeof(List<>))
                && typeof(ScriptableObject).IsAssignableFrom( valueType.GetGenericArguments()[0])){

                object[] attributes = fieldInfo.GetCustomAttributes(false);
                if (attributes == null || attributes.Length == 0){
                    return false;
                }
                GraphAttribute attribute =  attributes
                    .ToList().First((arg) => arg.GetType() == typeof(GraphAttribute)) as GraphAttribute;
                if (attribute != null){
                    graphData = new GraphData();
                    graphData.ItemBaseType = valueType.GetGenericArguments()[0];
                    graphData.ItemList = value as List<ScriptableObject>;
                    graphData.PropertyName = attribute.Name;
                    graphData.ParentObject = parentObject;
                    if (string.IsNullOrEmpty(graphData.PropertyName)){
                        graphData.PropertyName = fieldInfo.Name;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
