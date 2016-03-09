using UnityEngine;
using System.Collections;
using NodeInspector;


namespace NodeInspector.Demo{    
	[CreateAssetMenu]
    [Joint(JointType.Incognito_In)]
    [NodeMenuItem("IncognitoNode")]
    public class IncognitoNode : ScriptableObjectNode {
		[Joint(JointType.OneToOne_Incognito_OUT)]
        public IncognitoNode NextNode;


        public IncognitoNode UnusedNode;
    }    
}