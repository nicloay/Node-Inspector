using UnityEngine;
using UnityEditor;


namespace NodeInspector.Editor{
    public class JointData {

        const float KnobSize = 15.0f;
        const float BezierNormalMagnitude = 30.0f;
        Rect windowRect;
        public Rect         FieldInternalRect     {get; private set;}
		public JointType 	JointType			  {get; private set;}
        public Object       ObjectRefferenceValue {get; private set;}
		

        public GUIStyle KnobButtonStyle     {get; private set;}
        public Vector2  BezierNormal        {get; private set;}
        public Vector2  BezierSidePoint     {get; private set;}
        public Rect     KnobButtonRect      {get; private set;}
        public string   KnobButtonCaption   {get; private set;}


        public JointData(Object objectRefferenceValue, Rect fieldInternalRect, Rect windowRect, JointType jointType){
            ObjectRefferenceValue = objectRefferenceValue;
			FieldInternalRect = fieldInternalRect;
            JointType = jointType;
			this.windowRect = windowRect;
            SetupGUIVariables();
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