using UnityEngine;


namespace NodeInspector.Editor{
    public class JoinData {
        public Rect FieldInternalRect;//rect in window coordinates
        public JoinType JoinType;


        public JoinData(Rect fieldInternalRect, JoinType joinType){
            FieldInternalRect = fieldInternalRect;
            JoinType = joinType;
        }
    
    }
}