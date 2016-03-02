using UnityEngine;
using UnityEditor;


namespace NodeInspector.Editor{
    public class JointData {
		public Rect 			  FieldInternalRect  { get; private set;}
		Rect WindowRect;
		public JointType 		  JointType			 { get; private set;}
		public SerializedProperty SerializedProperty { get; private set;}
		public Object  				 RefObject	 { get; private set;}

		public Vector2 BezierSidePoint {
			get {
				return WindowRect.center;// FieldInternalRect.center;
			}
		}

		public Object SingleOutputObject{
			get {
				return SerializedProperty.objectReferenceValue;
			}
		}



		public JointData(SerializedProperty serializedProperty, Rect fieldInternalRect, Rect windowRect, JointType jointType){
			SerializedProperty = serializedProperty;
			FieldInternalRect = fieldInternalRect;
            JointType = jointType;
			WindowRect = windowRect;
        }
    

		public JointData(Object refObject, Rect fieldInternalRect, Rect windowRect, JointType jointType){
			RefObject = refObject;
			FieldInternalRect = fieldInternalRect;
			JointType = jointType;
			WindowRect = windowRect;
		}
    }
}