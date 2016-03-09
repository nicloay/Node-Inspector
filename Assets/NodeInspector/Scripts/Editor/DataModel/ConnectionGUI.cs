using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NodeInspector.Editor{	
	public class ConnectionGUI{
        const float BezierMinDistanceForSelection = 2.0f;

        public int ControlID {get; private set;}
        public JointData InputJoint  { get; set;}
		public JointData OutputJoint { get; set;}

        public ConnectionGUI(){
            ControlID = GUIUtility.GetControlID(FocusType.Passive);
        }

        public void OnGUI(){

            if (Event.current.type == EventType.mouseDown){                
                Debug.LogFormat ("{0} {1} {2} {3} {4} =>> {5}", Event.current.mousePosition, OutputJoint.BezierSidePoint, InputJoint.BezierSidePoint,
                    OutputJoint.BezierNormal, InputJoint.BezierNormal, HandleUtility.DistancePointBezier(Event.current.mousePosition, OutputJoint.BezierSidePoint, InputJoint.BezierSidePoint,
                        OutputJoint.BezierNormal, InputJoint.BezierNormal));
                if (HandleUtility.DistancePointBezier(Event.current.mousePosition, OutputJoint.BezierSidePoint, InputJoint.BezierSidePoint,
                    OutputJoint.BezierNormal, InputJoint.BezierNormal) <= BezierMinDistanceForSelection){
                    Debug.LogFormat("close to bezier {0}", Event.current.type);
                    GUIUtility.hotControl = ControlID;
                    //Event.current.Use();
                }    
            }

            switch(Event.current.GetTypeForControl(ControlID)){
                case EventType.Repaint:
                    {                        
                        float width = GUIUtility.hotControl == ControlID ? 5.0f : 3.0f;
                        Color color = GUIUtility.hotControl == ControlID ? Color.black : Color.gray;

                        Handles.BeginGUI();
                        Handles.DrawBezier (OutputJoint.BezierSidePoint, InputJoint.BezierSidePoint,
                            OutputJoint.BezierNormal, InputJoint.BezierNormal, color, null, width);            
                        Handles.EndGUI();
                        break;
                    }                    
            }

           
        }
	}
}