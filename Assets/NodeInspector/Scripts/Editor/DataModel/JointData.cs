using UnityEngine;
using UnityEditor;


namespace NodeInspector.Editor{
    public class JointData {
        Rect WindowRect;
        public Rect         FieldInternalRect  { get; private set;}
		public JointType 	JointType			 { get; private set;}
        public Object       ObjectRefferenceValue { get; private set;}
		
		public Vector2 BezierSidePoint {
			get {
				return WindowRect.center;// FieldInternalRect.center;
			}
		}

        public JointData(Object objectRefferenceValue, Rect fieldInternalRect, Rect windowRect, JointType jointType){
            ObjectRefferenceValue = objectRefferenceValue;
			FieldInternalRect = fieldInternalRect;
            JointType = jointType;
			WindowRect = windowRect;
        }
    
    }
}