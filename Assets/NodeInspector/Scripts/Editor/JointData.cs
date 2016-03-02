using UnityEngine;


namespace NodeInspector.Editor{
    public class JointData {
        public Rect FieldInternalRect;//rect in window coordinates
        public JointType JointType;


        public JointData(Rect fieldInternalRect, JointType jointType){
            FieldInternalRect = fieldInternalRect;
            JointType = jointType;
        }
    
    }
}