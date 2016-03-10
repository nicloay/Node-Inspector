using UnityEngine;
using UnityEditor;


namespace NodeInspector.Editor{
    public class JointData {

        const float KnobSize = 15.0f;
        const float BezierNormalMagnitude = 30.0f;
        Rect windowRect;
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

        public static JointData GetInstance(int controlID, SerializedProperty serializedProperty, Rect fieldInternalRect, Rect windowRect, JointType jointType){
            JointData result = GetInstance(controlID, fieldInternalRect, windowRect, jointType);
            result.SerializedProperty = serializedProperty ;
            result.ObjectRefferenceValue = serializedProperty.objectReferenceValue;
            return result;
        }

        public static JointData GetInstance(int controlID, Object scriptableObject, Rect fieldInternalRect, Rect windowRect, JointType jointType){
            JointData result = GetInstance(controlID, fieldInternalRect, windowRect, jointType);
            result.ObjectRefferenceValue = scriptableObject;
            return result;
        }

        static JointData GetInstance(int controlID, Rect fieldInternalRect, Rect windowRect, JointType jointType){
            JointData result = (JointData)GUIUtility.GetStateObject(typeof(JointData), controlID);
            result.ControlID = controlID;
            if (Event.current.type == EventType.repaint){
                result.FieldInternalRect = fieldInternalRect;
                result.windowRect = windowRect;                
            }

            result.JointType = jointType;
            result.SetupGUIVariables();
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

        public void OnGUI(NodeInspector parentWindow){
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
                            parentWindow.Repaint();
                        }
                        break;
                    }
                case EventType.mouseDown:
                    {
                        if (KnobButtonRect.Contains(Event.current.mousePosition)){
                            GUIUtility.hotControl = ControlID;
                            Debug.Log("hot control set");
                            Event.current.Use();
                        }
                        break;
                    }
                case EventType.mouseUp:
                    {
                        if (GUIUtility.hotControl == ControlID){
                            GUIUtility.hotControl = 0;
                            Debug.Log("hot control unset");
                        }
                        break;
                    }                
                case EventType.MouseDrag:                    
                    {
                       
                        break;
                    }
            
            }


        }


        void SetupGUIVariables()
        {
            switch(JointType){
                case JointType.Incognito_In:
                    KnobButtonRect = new Rect(windowRect.x + KnobSize, windowRect.y - KnobSize/2.0f, KnobSize,KnobSize);
                    BezierSidePoint = KnobButtonRect.position+ new Vector2(KnobSize*0.5f, 0.0f);
                    BezierNormal = BezierSidePoint + Vector2.down * BezierNormalMagnitude;
                    break;
                case JointType.ManyToMany_IN:
                case JointType.ManyToOne_IN:
                case JointType.OneToMany_IN:
                case JointType.OneToOne_IN:
                    KnobButtonRect = new Rect(windowRect.x - KnobSize, windowRect.y + FieldInternalRect.y, KnobSize, KnobSize);
                    BezierSidePoint = KnobButtonRect.position+ new Vector2(0.0f, KnobSize*0.5f);
                    BezierNormal = BezierSidePoint + Vector2.left * BezierNormalMagnitude;
                    break;
                default:
                    KnobButtonRect = new Rect(windowRect.x + windowRect.width, windowRect.y + FieldInternalRect.y, KnobSize, KnobSize);
                    BezierSidePoint = KnobButtonRect.position+ new Vector2(KnobSize, KnobSize*0.5f);
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