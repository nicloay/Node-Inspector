using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NodeInspector.Editor{	
    public enum ConnectionRenderType{
        OutputToInput,
        MouseToInputNode,
        OutputNodeToMouse
    }

    public class Connection{
        const float BezierMinDistanceForSelection = 3.0f;
        const float BezierTangentLengthRatio = 0.25f;

        public int      ControlID   {get; private set;}
        public Joint    InputJoint  {get; set;}
		public Joint    OutputJoint {get; set;}
        public bool     Focused     {get; private set;}

        public ConnectionRenderType ConnectionType {get; set;}


        public static Connection GetInstance(){            
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Connection result = (Connection)GUIUtility.GetStateObject(typeof(Connection), controlID);
            result.ControlID = controlID;
            return result;
        }



        public void OnGUI(){

            if (Event.current.type == EventType.mouseDown && ConnectionType == ConnectionRenderType.OutputToInput){  
                Focused = false;             
                Vector2 startTangent,endTangent;
                GetBezierTangent(OutputJoint.BezierSidePoint,OutputJoint.BezierTangent
                    , InputJoint.BezierSidePoint, InputJoint.BezierTangent, out startTangent, out endTangent);
                if (HandleUtility.DistancePointBezier(Event.current.mousePosition, OutputJoint.BezierSidePoint, InputJoint.BezierSidePoint,
                    startTangent, endTangent) <= BezierMinDistanceForSelection){                   
                    GUIUtility.hotControl = ControlID;
                    Focused = true;
                    Event.current.Use();
                }    
            }


            switch(Event.current.GetTypeForControl(ControlID)){
                case EventType.Repaint:
                    {                 
                        Focused = Focused && ControlID == GUIUtility.hotControl;
                        float width = 2.0f;
                        Color color = Focused ? Color.black : Color.gray;

                        Handles.BeginGUI();

                        switch (ConnectionType){
                            case ConnectionRenderType.OutputToInput:
                                {
                                    DrawBezier(OutputJoint.BezierSidePoint, OutputJoint.BezierTangent, InputJoint.BezierSidePoint, InputJoint.BezierTangent, color, width);                                    
                                    break;
                                }
                            case ConnectionRenderType.MouseToInputNode:
                                {
                                    Vector2 srcPosition = Event.current.mousePosition;

                                    DrawBezier(srcPosition, InputJoint.BezierBackTangent
                                        , InputJoint.BezierSidePoint, InputJoint.BezierTangent, color, width );                                    
                                    break;
                                }
                            case ConnectionRenderType.OutputNodeToMouse:
                                {
                                    Vector2 dstPosition = Event.current.mousePosition;
                                    DrawBezier(OutputJoint.BezierSidePoint, OutputJoint.BezierTangent
                                        , dstPosition, OutputJoint.BezierBackTangent, color, width);
                                    break;
                                }
                        }


                        Handles.EndGUI();
                        break;
                    }                   
                case EventType.KeyDown:
                    {
                        if (GUIUtility.hotControl == ControlID 
                            && (Event.current.keyCode == KeyCode.Delete || Event.current.keyCode == KeyCode.Backspace)){
                            InputJoint.ClearJointLink();
                            OutputJoint.ClearJointLink();
                            GUIUtility.hotControl = 0;
                            Event.current.Use();
                        }
                        break;
                    }
            }
        }
	
        void DrawBezier(Vector2 startPoint, Vector2 startTangent, Vector2 endPoint, Vector2 endTangent, Color color, float width){
            Vector2 startTangentLocation, endTangentLocation;
            GetBezierTangent(startPoint, startTangent, endPoint, endTangent, out startTangentLocation, out endTangentLocation);
            Handles.DrawBezier(startPoint, endPoint, startTangentLocation, endTangentLocation
                , color, null, width);
        }

        void GetBezierTangent(Vector2 startPoint, Vector2 startTangent, Vector2 endPoint, 
            Vector2 endTangent, out Vector2 startTangentLocation, out Vector2 endTangentLocation){
            float tangentMagnitude =  (endPoint - startPoint).magnitude * BezierTangentLengthRatio;
            startTangentLocation = startPoint+ startTangent * tangentMagnitude;
            endTangentLocation = endPoint + endTangent * tangentMagnitude;                
        }
    }
}