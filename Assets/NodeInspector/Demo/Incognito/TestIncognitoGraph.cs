using UnityEngine;
using System.Collections.Generic;


namespace NodeInspector.Demo{  
    [CreateAssetMenu]
    public class TestIncognitoGraph : ScriptableObject {
        [Graph]
        public List<IncognitoNode> IncognitorNodes;
    }    
}