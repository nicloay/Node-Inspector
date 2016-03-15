using UnityEngine;
using System.Collections.Generic;
using NodeInspector;

namespace NodeInspector.Editor{
    public static class ObjectConnector {



        public static void CreateNewConnection(Joint joint1, Joint joint2){
            JointType type1 = joint1.JointType;
            JointType type2 = joint2.JointType;

            if (CouldConnectTwoJointsWithTypes(type1,type2)){

                //anonimous connections;
                if (type1 == JointType.Incognito_In){
                    joint2.SerializedProperty.objectReferenceValue = joint1.ObjectRefferenceValue;
                    joint2.SerializedProperty.serializedObject.ApplyModifiedProperties();
                } else {
                    joint1.SerializedProperty.objectReferenceValue = joint2.ObjectRefferenceValue;
                    joint1.SerializedProperty.serializedObject.ApplyModifiedProperties();
                }
                GUI.changed = true;
            } 
        }





        public static bool CouldConnectTwoJointsWithTypes(JointType type1, JointType type2){
            if (!AcceptedJointsDB.ContainsKey(type1) || !AcceptedJointsDB.ContainsKey(type2)){
                Debug.LogErrorFormat("FIXME: usuported joint type {0} or {1}", type1, type2);
                return false;
            }
            return AcceptedJointsDB[type1].Contains(type2);
        }

        static Dictionary<JointType, HashSet<JointType>> AcceptedJointsDB = new Dictionary<JointType, HashSet<JointType>>{
            {JointType.Incognito_In, new HashSet<JointType>(){JointType.OneToOne_Incognito_OUT, JointType.ManyToOne_Incognito_OUT}},
            {JointType.OneToOne_Incognito_OUT, new HashSet<JointType>(){JointType.Incognito_In}},
            {JointType.ManyToOne_Incognito_OUT, new HashSet<JointType>(){JointType.Incognito_In}}
        };
    }
}