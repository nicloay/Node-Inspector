using UnityEngine;
using System.Collections.Generic;
using NodeInspector;

namespace NodeInspector.Editor{
    public static class ObjectConnector {
        public static void TryToMakeNewConnection(Joint from, Joint to, Connection lastDraggedConnection)
        {
            if (from != null && to != null){                
                if (lastDraggedConnection.InputJoint != null && lastDraggedConnection.OutputJoint != null){
                    ObjectConnector.UpdateExistingConnection(from, to, lastDraggedConnection);
                } else {
                    ObjectConnector.CreateNewConnection(from, to);
                }
            }
        }


        public static void CreateNewConnection(Joint joint1, Joint joint2){
            JointType type1 = joint1.JointType;
            JointType type2 = joint2.JointType;

            if (CouldConnectTwoJointsWithTypes(type1,type2)){

                //anonimous connections;
                if (type1 == JointType.OneWay_IN){
                    joint2.SerializedProperty.objectReferenceValue = joint1.ObjectRefferenceValue;
                    joint2.SerializedProperty.serializedObject.ApplyModifiedProperties();
                } else {
                    joint1.SerializedProperty.objectReferenceValue = joint2.ObjectRefferenceValue;
                    joint1.SerializedProperty.serializedObject.ApplyModifiedProperties();
                }
                GUI.changed = true;
            } 
        }


        public static void UpdateExistingConnection(Joint jointFrom, Joint jointTo, Connection existingConnection){
            if (jointFrom == jointTo){
                return;
            }
            Joint changeJointFrom, changeJointTo;
            Joint anotherJoint;
            if (existingConnection.InputJoint == jointFrom || existingConnection.InputJoint == jointTo){
                changeJointFrom = existingConnection.InputJoint;
                anotherJoint = existingConnection.OutputJoint;
            } else {
                changeJointFrom = existingConnection.OutputJoint;
                anotherJoint = existingConnection.InputJoint;
            }
            ClearReffToAnotherObject(changeJointFrom);
            changeJointTo = jointFrom == changeJointFrom ? jointTo : jointFrom;
            CreateNewConnection(changeJointTo, anotherJoint);
        }

        static void ClearReffToAnotherObject(Joint joint){
            switch (joint.JointType){                
                case JointType.OneWay_OUT:
                    joint.SerializedProperty.objectReferenceValue = null;
                    joint.SerializedProperty.serializedObject.ApplyModifiedProperties();
                    GUI.changed = true;
                    break;
                case JointType.OneWay_IN:
                    break;//do nothing
                default:
                    Debug.LogError("Unsuported type "+joint.JointType) ;
                    break;
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
            {JointType.OneWay_IN , new HashSet<JointType>(){JointType.OneWay_OUT}},
            {JointType.OneWay_OUT, new HashSet<JointType>(){JointType.OneWay_IN}}
        };
    }
}