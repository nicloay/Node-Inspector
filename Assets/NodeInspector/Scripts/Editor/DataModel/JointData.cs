using UnityEngine;
using UnityEditor;


namespace NodeInspector.Editor{
    public class JointData {


        const float BezierNormalMagnitude = 30.0f;
        public Rect               FieldInternalRect     {get; private set;}
		public JointType 	      JointType			    {get; private set;}
        public SerializedProperty SerializedProperty    {get; private set;} //we have this property if it's link to property
        public Object             ObjectRefferenceValue {get; private set;}  //value or link to anonimous class


        public GUIStyle KnobButtonStyle     {get; private set;}
        public Vector2  BezierNormal        {get; private set;}
        public Vector2  BezierSidePoint     {get; private set;}
        public Rect     KnobButtonRect      {get; private set;}
        public string   KnobButtonCaption   {get; private set;}

        public int ControlID {get; private set;}

        public static JointData GetInstance(SerializedProperty serializedProperty, Rect fieldInternalRect, JointType jointType, Vector2 parentWindowGlobalPosition){
            JointData result = GetInstance(fieldInternalRect, jointType, parentWindowGlobalPosition);
            result.SerializedProperty = serializedProperty ;
            result.ObjectRefferenceValue = serializedProperty.objectReferenceValue;
            return result;
        }

        public static JointData GetInstance(Object scriptableObject, Rect fieldInternalRect, JointType jointType, Vector2 parentWindowGlobalPosition){
            JointData result = GetInstance(fieldInternalRect, jointType,parentWindowGlobalPosition);
            result.ObjectRefferenceValue = scriptableObject;
            return result;
        }

        static JointData GetInstance( Rect fieldInternalRect, JointType jointType, Vector2 parentWindowGlobalPosition){
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            JointData result = (JointData)GUIUtility.GetStateObject(typeof(JointData), controlID);
            result.ControlID = controlID;
            if (Event.current.type == EventType.repaint){
                result.FieldInternalRect = fieldInternalRect;                              
            }

            result.JointType = jointType;
            result.SetupGUIVariables(parentWindowGlobalPosition);
            return result;
        }

        public void ClearJointLink(){
            
            switch(JointType){
                case JointType.OneToOne_Incognito_OUT:
                    SerializedProperty.serializedObject.Update();
                    SerializedProperty.objectReferenceValue = null;
                    SerializedProperty.serializedObject.ApplyModifiedProperties();

                    GUI.changed = true;
                    break;
                case JointType.Incognito_In:
                    //don't need to clear anything;
                    break;
                default:
                    Debug.LogError("Unsupported type");
                    break;
            }
        }

        public void OnGUI(/*NodeInspector parentWindow*/){
            switch (Event.current.GetTypeForControl(ControlID)){
                case EventType.Repaint:
                    {
                        GUI.Button(KnobButtonRect, KnobButtonCaption, KnobButtonStyle);
                        if (GUIUtility.hotControl == ControlID){                            
                            Debug.Log("wtf ??? ");
                            Vector2 mousePosition = Event.current.mousePosition;
                            Vector2 mouseNormal =mousePosition + (BezierSidePoint - mousePosition).normalized * 50.0f;
                            Debug.LogFormat("{0} {1} {2} {3}",BezierSidePoint, mousePosition,
                                BezierNormal, mouseNormal);
                            Handles.BeginGUI();
                            Handles.DrawBezier (BezierSidePoint, mousePosition,
                                BezierNormal, mouseNormal, Color.gray, null, 3.0f);            
                            Handles.EndGUI();
                            //parentWindow.Repaint();
                        }
                        break;
                    }
                case EventType.mouseDown:
                    {
                        if (KnobButtonRect.Contains(Event.current.mousePosition)){
                            GUIUtility.hotControl = ControlID;
                            Event.current.Use();
                        }
                        break;
                    }
                case EventType.mouseUp:
                    {
                        if (GUIUtility.hotControl == ControlID){
                            GUIUtility.hotControl = 0;
                        }
                        break;
                    }                
                case EventType.MouseDrag:                    
                    {
                       
                        break;
                    }           
            }
        }


        void SetupGUIVariables(Vector2 parentWindowGlobalPosition)
        {
            switch(JointType){
                case JointType.Incognito_In:
                    KnobButtonRect = new Rect(NodeGUI.KnobSize * 2,  0, NodeGUI.KnobSize, NodeGUI.KnobSize);
                    BezierSidePoint = parentWindowGlobalPosition + new Vector2(NodeGUI.KnobSize*2.5f, 0.0f);
                    BezierNormal = BezierSidePoint + Vector2.down * BezierNormalMagnitude;
                    break;
                case JointType.ManyToMany_IN:
                case JointType.ManyToOne_IN:
                case JointType.OneToMany_IN:
                case JointType.OneToOne_IN:
                    KnobButtonRect = new Rect(0, FieldInternalRect.y, NodeGUI.KnobSize, NodeGUI.KnobSize);
                    BezierSidePoint = parentWindowGlobalPosition + KnobButtonRect.position + new Vector2(0.0f, NodeGUI.KnobSize*0.5f);
                    BezierNormal = BezierSidePoint + Vector2.left * BezierNormalMagnitude;
                    break;
                default:
                    KnobButtonRect = new Rect( FieldInternalRect.x + FieldInternalRect.width + NodeGUI.OriginalRightPatdding, FieldInternalRect.y, NodeGUI.KnobSize, NodeGUI.KnobSize);
                    BezierSidePoint = parentWindowGlobalPosition + KnobButtonRect.position+ new Vector2(NodeGUI.KnobSize, NodeGUI.KnobSize*0.5f);
                    BezierNormal = BezierSidePoint + Vector2.right * BezierNormalMagnitude;
                    break;
            }

            switch(JointType){
                case JointType.Incognito_In:
                    KnobButtonCaption = "i";
                    break;
                case JointType.ManyToMany_IN:
                case JointType.ManyToOne_IN:
                case JointType.OneToMany_OUT:
                case JointType.ManyToMany_OUT:                        
                    KnobButtonCaption = "∞";
                    break;
                default:
                    KnobButtonCaption = "1";
                    break;                
            }

            switch(JointType){
                case JointType.Incognito_In:
                    KnobButtonStyle = EditorStyles.miniButton;
                    break;
                case JointType.ManyToMany_IN:
                case JointType.ManyToOne_IN:
                case JointType.OneToMany_IN:
                case JointType.OneToOne_IN:
                    KnobButtonStyle = EditorStyles.miniButtonLeft;
                    break;
                default:
                    KnobButtonStyle = EditorStyles.miniButtonRight;
                    break;   
            }
        }


           

    }
}