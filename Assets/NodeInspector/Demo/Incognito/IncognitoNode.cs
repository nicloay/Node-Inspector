using UnityEngine;
using System.Collections;
using NodeInspector;


namespace NodeInspector.Demo{    
    [Join(JoinType.Incognito_In)]
    [NodeMenuItem("IncognitoNode")]
    public class IncognitoNode : ScriptableObjectNode {
        [Join(JoinType.OneToOne_OUT)]
        public IncognitoNode NextNode;

        public IncognitoNode UnusedNode;
    }    
}